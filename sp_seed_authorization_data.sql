CREATE PROCEDURE [dbo].[sp_seed_authorization_data]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    DECLARE @UtcNow DATETIME2 = SYSUTCDATETIME();

    -- ROLES
    IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'User')
    BEGIN
        INSERT INTO Roles (Id, Name, CreatedOn)
        VALUES (NewId(), 'User', @UtcNow);
    END

    IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Admin')
    BEGIN
        INSERT INTO Roles (Id, Name, CreatedOn)
        VALUES (NewId(), 'Admin', @UtcNow);
    END

    -- PERMISSIONS
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'VIEW_NEARBY_PRODUCTS')
        INSERT INTO Permissions (Id, Name, CreatedOn)
        VALUES (NewId(), 'VIEW_NEARBY_PRODUCTS', @UtcNow);

    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'CREATE_PRODUCT')
        INSERT INTO Permissions (Id, Name, CreatedOn)
        VALUES (NewId(), 'CREATE_PRODUCT', @UtcNow);

    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'SWIPE_PRODUCT')
        INSERT INTO Permissions (Id, Name, CreatedOn)
        VALUES (NewId(), 'SWIPE_PRODUCT', @UtcNow);

    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'WISHLIST_PRODUCT')
        INSERT INTO Permissions (Id, Name, CreatedOn)
        VALUES (NewId(), 'WISHLIST_PRODUCT', @UtcNow);

    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'CHAT')
        INSERT INTO Permissions (Id, Name, CreatedOn)
        VALUES (NewId(), 'CHAT', @UtcNow);

    -- ROLE → PERMISSION MAPPING
    DECLARE @UserRoleId UNIQUEIDENTIFIER =
        (SELECT Id FROM Roles WHERE Name = 'User');

    INSERT INTO RolePermissions (RoleId, PermissionId, CreatedOn)
    SELECT @UserRoleId, p.Id, @UtcNow
    FROM Permissions p
    WHERE p.Name IN (
        'VIEW_NEARBY_PRODUCTS',
        'CREATE_PRODUCT',
        'SWIPE_PRODUCT',
        'WISHLIST_PRODUCT',
        'CHAT'
    )
    AND NOT EXISTS (
        SELECT 1 FROM RolePermissions rp
        WHERE rp.RoleId = @UserRoleId
          AND rp.PermissionId = p.Id
    );

    COMMIT TRANSACTION;
END
GO