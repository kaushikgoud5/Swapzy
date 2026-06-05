-- Seed Authorization Data (Roles, Permissions, RolePermissions)
-- Run this in Supabase SQL Editor after applying EF migrations

DO $$
DECLARE
    v_utc_now TIMESTAMP WITH TIME ZONE := NOW();
    v_user_role_id UUID;
BEGIN

    -- ROLES
    INSERT INTO "Roles" ("Id", "Name", "CreatedOn")
    SELECT gen_random_uuid(), 'User', v_utc_now
    WHERE NOT EXISTS (SELECT 1 FROM "Roles" WHERE "Name" = 'User');

    INSERT INTO "Roles" ("Id", "Name", "CreatedOn")
    SELECT gen_random_uuid(), 'Admin', v_utc_now
    WHERE NOT EXISTS (SELECT 1 FROM "Roles" WHERE "Name" = 'Admin');

    -- PERMISSIONS
    INSERT INTO "Permissions" ("Id", "Name", "CreatedOn")
    SELECT gen_random_uuid(), 'VIEW_NEARBY_PRODUCTS', v_utc_now
    WHERE NOT EXISTS (SELECT 1 FROM "Permissions" WHERE "Name" = 'VIEW_NEARBY_PRODUCTS');

    INSERT INTO "Permissions" ("Id", "Name", "CreatedOn")
    SELECT gen_random_uuid(), 'CREATE_PRODUCT', v_utc_now
    WHERE NOT EXISTS (SELECT 1 FROM "Permissions" WHERE "Name" = 'CREATE_PRODUCT');

    INSERT INTO "Permissions" ("Id", "Name", "CreatedOn")
    SELECT gen_random_uuid(), 'SWIPE_PRODUCT', v_utc_now
    WHERE NOT EXISTS (SELECT 1 FROM "Permissions" WHERE "Name" = 'SWIPE_PRODUCT');

    INSERT INTO "Permissions" ("Id", "Name", "CreatedOn")
    SELECT gen_random_uuid(), 'WISHLIST_PRODUCT', v_utc_now
    WHERE NOT EXISTS (SELECT 1 FROM "Permissions" WHERE "Name" = 'WISHLIST_PRODUCT');

    INSERT INTO "Permissions" ("Id", "Name", "CreatedOn")
    SELECT gen_random_uuid(), 'CHAT', v_utc_now
    WHERE NOT EXISTS (SELECT 1 FROM "Permissions" WHERE "Name" = 'CHAT');

    -- ROLE -> PERMISSION MAPPING
    SELECT "Id" INTO v_user_role_id FROM "Roles" WHERE "Name" = 'User';

    INSERT INTO "RolePermissions" ("RoleId", "PermissionId", "CreatedOn")
    SELECT v_user_role_id, p."Id", v_utc_now
    FROM "Permissions" p
    WHERE p."Name" IN (
        'VIEW_NEARBY_PRODUCTS',
        'CREATE_PRODUCT',
        'SWIPE_PRODUCT',
        'WISHLIST_PRODUCT',
        'CHAT'
    )
    AND NOT EXISTS (
        SELECT 1 FROM "RolePermissions" rp
        WHERE rp."RoleId" = v_user_role_id
          AND rp."PermissionId" = p."Id"
    );

END $$;
