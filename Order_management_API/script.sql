CREATE PROCEDURE dbo.GetOrdersByPage
    @PageIndex INT,
    @PageSize INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.Orders
    ORDER BY OrderDate
    OFFSET (@PageIndex - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
