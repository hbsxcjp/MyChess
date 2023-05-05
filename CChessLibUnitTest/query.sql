-- sqlite

-- DELETE FROM manual WHERE id > 10;
-- UPDATE sqlite_sequence SET seq = 100000;
SELECT * FROM sqlite_sequence;

SELECT count(*) FROM manual;
SELECT id, source, title FROM manual WHERE id < 11;
SELECT id, source, title FROM manual WHERE id > 112140;

-- VACUUM ;

