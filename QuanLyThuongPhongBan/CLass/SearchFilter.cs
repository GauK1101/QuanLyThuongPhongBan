using System.Linq.Expressions;

namespace QuanLyThuongPhongBan.CLass
{
    internal class SearchFilter
    {
        public static class SearchViewModel
        {
            public static IQueryable<T> Search<T>(IQueryable<T> query, string search) where T : class
            {
                if (string.IsNullOrEmpty(search))
                {
                    return query;
                }

                // Tách các điều kiện tìm kiếm bởi dấu phẩy
                string[] searchTerms = search.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                // Lấy tất cả các thuộc tính của lớp T (giả sử là DonHangVaSanPham)
                var properties = typeof(T).GetProperties();

                // Biểu thức tổng hợp kết hợp các điều kiện
                Expression? combinedPredicate = null;
                var parameter = Expression.Parameter(typeof(T), "d");

                foreach (var term in searchTerms)
                {
                    Expression? termPredicate = null;

                    // Kiểm tra xem điều kiện tìm kiếm có chỉ định cột cụ thể không
                    if (!term.Contains(":"))
                    {
                        // Nếu không chỉ định cột, tìm kiếm trên tất cả các thuộc tính chuỗi
                        foreach (var prop in properties)
                        {
                            if (prop.PropertyType == typeof(string))
                            {
                                var propertyAccess = Expression.Property(parameter, prop);
                                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                                var containsExpression = Expression.Call(propertyAccess, containsMethod, Expression.Constant(term.Trim().ToLower()));

                                termPredicate = termPredicate == null
                                    ? containsExpression
                                    : Expression.Or(termPredicate, containsExpression);
                            }
                        }
                    }
                    else
                    {
                        // Nếu chỉ định cột cụ thể (column:searchTerm)
                        string[] parts = term.Split(':');
                        string column = parts[0].ToLower().Trim();
                        string value = parts[1].Trim().ToLower();

                        // Tìm thuộc tính tương ứng với cột được chỉ định
                        var prop = properties.FirstOrDefault(p => p.Name.ToLower() == column);
                        if (prop == null) continue;

                        var propertyAccess = Expression.Property(parameter, prop);

                        // Kiểm tra kiểu dữ liệu và tạo điều kiện tìm kiếm phù hợp
                        if (prop.PropertyType == typeof(string))
                        {
                            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                            var containsExpression = Expression.Call(propertyAccess, containsMethod, Expression.Constant(value));
                            termPredicate = containsExpression;
                        }
                    }

                    // Kết hợp các điều kiện
                    if (combinedPredicate == null)
                    {
                        combinedPredicate = termPredicate;
                    }
                    else
                    {
                        combinedPredicate = Expression.AndAlso(combinedPredicate, termPredicate);
                    }
                }

                if (combinedPredicate != null)
                {
                    var lambda = Expression.Lambda<Func<T, bool>>(combinedPredicate, parameter);
                    query = query.Where(lambda);
                }

                return query;
            }
        }
    }
}
