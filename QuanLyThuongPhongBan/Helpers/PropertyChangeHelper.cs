using System.Reflection;

namespace QuanLyThuongPhongBan.Helpers
{
    public class PropertyChange
    {
        public string PropertyName { get; set; } = string.Empty;
        public object? OriginalValue { get; set; }
        public object? CurrentValue { get; set; }
        public Type PropertyType { get; set; } = typeof(object);
        public List<PropertyChange>? ChildChanges { get; set; }
    }

    public static class PropertyChangeHelper
    {
        public static List<PropertyChange> GetChangedProperties<T>(T original, T current) where T : class
        {
            var changes = new List<PropertyChange>();

            if (original == null || current == null)
                return changes;

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                     .Where(p => p.CanRead);

            foreach (var prop in properties)
            {
                var originalValue = prop.GetValue(original);
                var currentValue = prop.GetValue(current);

                // Kiểm tra collection navigation properties (SmbTeamBonuses)
                if (typeof(System.Collections.IEnumerable).IsAssignableFrom(prop.PropertyType) &&
                    prop.PropertyType != typeof(string))
                {
                    var childChanges = GetCollectionChanges(originalValue, currentValue, prop.Name);
                    if (childChanges.Any())
                    {
                        changes.Add(new PropertyChange
                        {
                            PropertyName = prop.Name,
                            OriginalValue = originalValue,
                            CurrentValue = currentValue,
                            PropertyType = prop.PropertyType,
                            ChildChanges = childChanges
                        });
                    }
                }
                // Kiểm tra primitive properties
                else if (!IsNavigationProperty(prop))
                {
                    if (!Equals(originalValue, currentValue))
                    {
                        changes.Add(new PropertyChange
                        {
                            PropertyName = prop.Name,
                            OriginalValue = originalValue,
                            CurrentValue = currentValue,
                            PropertyType = prop.PropertyType
                        });
                    }
                }
            }

            return changes;
        }

        private static List<PropertyChange> GetCollectionChanges(object? original, object? current, string parentName)
        {
            var changes = new List<PropertyChange>();

            if (original is System.Collections.IEnumerable originalCollection &&
                current is System.Collections.IEnumerable currentCollection)
            {
                var originalList = originalCollection.Cast<object>().ToList();
                var currentList = currentCollection.Cast<object>().ToList();

                // Kiểm tra số lượng item thay đổi
                if (originalList.Count != currentList.Count)
                {
                    changes.Add(new PropertyChange
                    {
                        PropertyName = $"{parentName}.Count",
                        OriginalValue = originalList.Count,
                        CurrentValue = currentList.Count,
                        PropertyType = typeof(int)
                    });
                }

                // Kiểm tra từng item trong collection
                for (int i = 0; i < Math.Min(originalList.Count, currentList.Count); i++)
                {
                    var itemChanges = GetObjectChanges(originalList[i], currentList[i], $"{parentName}[{i}]");
                    changes.AddRange(itemChanges);
                }
            }

            return changes;
        }

        private static List<PropertyChange> GetObjectChanges(object? original, object? current, string prefix)
        {
            var changes = new List<PropertyChange>();

            if (original == null || current == null || original.GetType() != current.GetType())
                return changes;

            var properties = original.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                   .Where(p => p.CanRead && !IsNavigationProperty(p));

            var departmentDisplayProp = original.GetType().GetProperty("DepartmentId");
            if (departmentDisplayProp != null)
            {
                var departmentName = departmentDisplayProp.GetValue(original) ?? departmentDisplayProp.GetValue(current);
                if (departmentName != null)
                {
                    prefix = $"{departmentName}";
                }
            }

            foreach (var prop in properties)
            {
                // Skip DepartmentDisplayName vì đã xử lý ở trên
                if (prop.Name == "DepartmentId")
                    continue;

                var originalValue = prop.GetValue(original);
                var currentValue = prop.GetValue(current);

                if (!Equals(originalValue, currentValue))
                {
                    changes.Add(new PropertyChange
                    {
                        PropertyName = $"{prefix}.{prop.Name}",
                        OriginalValue = originalValue,
                        CurrentValue = currentValue,
                        PropertyType = prop.PropertyType
                    });
                }
            }

            return changes;
        }

        private static bool IsNavigationProperty(PropertyInfo prop)
        {
            // Chỉ bỏ qua reference navigation properties, không bỏ qua collections
            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(prop.PropertyType) &&
                prop.PropertyType != typeof(string))
                return false; // KHÔNG bỏ qua collections

            // Bỏ qua reference navigation properties (các class khác)
            if (prop.PropertyType.IsClass &&
                prop.PropertyType != typeof(string) &&
                !prop.PropertyType.IsValueType)
                return true;

            return false;
        }

        public static bool HasAnyChanges<T>(T original, T current) where T : class
        {
            return GetChangedProperties(original, current).Any();
        }

        public static string GetChangesSummary<T>(T original, T current) where T : class
        {
            var summary = new System.Text.StringBuilder();
            var changes = GetChangedProperties(original, current);

            if (!changes.Any())
            {
                summary.AppendLine("Không có thay đổi nào.");
                return summary.ToString();
            }

            summary.AppendLine($"Đã phát hiện {GetTotalChangesCount(changes)} thay đổi:");

            foreach (var change in changes)
            {
                AppendChangeSummary(summary, change, 0);
            }

            return summary.ToString();
        }

        private static int GetTotalChangesCount(List<PropertyChange> changes)
        {
            int count = 0;
            foreach (var change in changes)
            {
                count++; // Count the parent change
                if (change.ChildChanges != null)
                {
                    count += change.ChildChanges.Count;
                }
            }
            return count;
        }

        private static void AppendChangeSummary(System.Text.StringBuilder summary, PropertyChange change, int indentLevel)
        {
            var indent = new string(' ', indentLevel * 2);

            // Lấy display name thay vì property name
            var displayName = GetDisplayNameForChange(change);

            if (change.ChildChanges == null || !change.ChildChanges.Any())
            {
                summary.AppendLine($"{indent}• {displayName}: '{change.OriginalValue}' → '{change.CurrentValue}'");
            }
            else
            {
                summary.AppendLine($"{indent}• {displayName}:");
                foreach (var childChange in change.ChildChanges)
                {
                    AppendChangeSummary(summary, childChange, indentLevel + 1);
                }
            }
        }

        private static string GetDisplayNameForChange(PropertyChange change)
        {
            // Xử lý cho nested properties (SmbTeamBonuses[0].Phase1Value)
            if (change.PropertyName.Contains("."))
            {
                var parts = change.PropertyName.Split('.');
                var propertyName = parts.Last();
                var departmentName = parts[0]; ;

                // Lấy display name cho property
                var displayName = GetPropertyDisplayName(propertyName, typeof(SmbTeamBonus));

                return $"{departmentName}: {GetPropertyDisplayName(propertyName, typeof(SmbTeamBonus))}";
            }
            else
            {
                // Property thông thường
                return GetPropertyDisplayName(change.PropertyName, typeof(SmbBonus));
            }
        }

        // Helper method để lấy DisplayAttribute
        private static string GetPropertyDisplayName(string propertyName, Type type)
        {
            var prop = type.GetProperty(propertyName);
            if (prop != null)
            {
                var displayAttr = prop.GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>();
                if (displayAttr != null)
                    return displayAttr.Name;
            }
            return propertyName;
        }
    }
}