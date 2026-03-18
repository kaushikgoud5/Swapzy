CREATE  PROCEDURE [dbo].[SeedMasterCategories]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Now DATETIME2 = GETUTCDATE();

    MERGE Categories AS target
    USING (
        SELECT
            'Electronics'              AS Name, 'electronics'              AS Slug, 'Mobile phones, laptops and gadgets' AS Description UNION ALL
        SELECT
            'Home & Kitchen',             'home-kitchen',             'Home appliances and kitchen items' UNION ALL
        SELECT
            'Fashion',                    'fashion',                  'Clothing and fashion accessories' UNION ALL
        SELECT
            'Books',                      'books',                    'Books and study materials' UNION ALL
        SELECT
            'Sports & Fitness',           'sports-fitness',           'Sports and fitness equipment' UNION ALL
        SELECT
            'Vehicles',                   'vehicles',                 'Bikes, cars and vehicle accessories' UNION ALL
        SELECT
            'Furniture',                  'furniture',                'Household and office furniture' UNION ALL
        SELECT
            'Toys & Games',               'toys-games',               'Toys and kids items' UNION ALL
        SELECT
            'Beauty & Personal Care',     'beauty-personal-care',     'Cosmetics and personal care products' UNION ALL
        SELECT
            'Others',                     'others',                   'Miscellaneous items'
    ) AS source
    ON target.Slug = source.Slug

    WHEN MATCHED AND (
        target.Name        <> source.Name OR
        ISNULL(target.Description, '') <> ISNULL(source.Description, '')
    )
    THEN
        UPDATE SET
            Name = source.Name,
            Description = source.Description,
            IsActive = 1,
            ModifiedOn = @Now

    WHEN NOT MATCHED BY TARGET
    THEN
        INSERT (
            Name,
            Slug,
            Description,
            IsActive,
            CreatedOn
        )
        VALUES (
            
            source.Name,
            source.Slug,
            source.Description,
            1,
            @Now
        );

END;
GO
