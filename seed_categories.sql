-- Seed Master Categories
-- Run this in Supabase SQL Editor after applying EF migrations

DO $$
DECLARE
    v_now TIMESTAMP WITH TIME ZONE := NOW();
BEGIN

    INSERT INTO "Categories" ("Name", "Slug", "Description", "IsActive", "CreatedOn")
    VALUES
        ('Electronics',             'electronics',            'Mobile phones, laptops and gadgets',       TRUE, v_now),
        ('Home & Kitchen',          'home-kitchen',           'Home appliances and kitchen items',        TRUE, v_now),
        ('Fashion',                 'fashion',                'Clothing and fashion accessories',         TRUE, v_now),
        ('Books',                   'books',                  'Books and study materials',                TRUE, v_now),
        ('Sports & Fitness',        'sports-fitness',         'Sports and fitness equipment',             TRUE, v_now),
        ('Vehicles',                'vehicles',               'Bikes, cars and vehicle accessories',      TRUE, v_now),
        ('Furniture',               'furniture',              'Household and office furniture',           TRUE, v_now),
        ('Toys & Games',            'toys-games',             'Toys and kids items',                      TRUE, v_now),
        ('Beauty & Personal Care',  'beauty-personal-care',   'Cosmetics and personal care products',    TRUE, v_now),
        ('Others',                  'others',                 'Miscellaneous items',                      TRUE, v_now)
    ON CONFLICT ("Slug") DO UPDATE SET
        "Name"        = EXCLUDED."Name",
        "Description" = EXCLUDED."Description",
        "IsActive"    = TRUE,
        "ModifiedOn"  = v_now;

END $$;
