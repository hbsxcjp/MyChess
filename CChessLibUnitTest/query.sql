-- sqlite

-- DELETE FROM manual WHERE id < 11;
-- DELETE FROM manual WHERE id > 112141;
-- UPDATE sqlite_sequence SET seq = 0 WHERE name = 'history';
-- SELECT * FROM sqlite_master;
-- SELECT * FROM sqlite_sequence;

-- SELECT count(*) FROM manual;
-- SELECT * FROM manual WHERE id < 11;
-- SELECT id, source, title FROM manual WHERE id < 11;
-- SELECT id, source, title FROM manual WHERE id > 112140;
SELECT * FROM manual WHERE id > 112140;

-- CREATE TABLE history (id INTEGER PRIMARY KEY AUTOINCREMENT, key INTEGER, lock INTEGER, frequency INTEGER);
-- DELETE FROM history;
-- SELECT count(*) FROM history;
-- SELECT * FROM history WHERE key = 9223372036854775807;

-- VACUUM ;

