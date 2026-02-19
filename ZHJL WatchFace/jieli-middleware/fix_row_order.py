import sqlite3

db_path = r'D:\Downloads\WatchFace\jieli-middleware\temp\方形_240x284_普通_201#简约_My Watch Face_001_00\watch.db'

conn = sqlite3.connect(db_path)
cursor = conn.cursor()

print("Current state:")
cursor.execute('SELECT ID, GroupCode FROM WatchGroups WHERE ID >= 33 ORDER BY ID')
print("Rows 33+:", cursor.fetchall())

# Step 1: Delete the row at ID 60
cursor.execute('DELETE FROM WatchGroups WHERE ID = 60')
print("\n✓ Deleted row at ID 60")

# Step 2: Shift IDs 34-59 up by 1 (do in descending order to avoid conflicts)
# First, temporarily move them to negative IDs to avoid conflicts
cursor.execute('UPDATE WatchGroups SET ID = -ID WHERE ID >= 34')

# Then move them back with +1 offset
cursor.execute('UPDATE WatchGroups SET ID = (-ID) + 1 WHERE ID < 0')
print("✓ Shifted rows 34-59 to 35-60")

# Step 3: Get the 0701 data and insert at ID 34
cursor.execute('SELECT WatchTypeId, GroupCode, Left, Top, ColorDesc FROM WatchGroups WHERE ID = 33')
row_33 = cursor.fetchone()

insert_sql = '''
INSERT INTO WatchGroups (ID, WatchTypeId, GroupCode, Left, Top, ColorDesc)
VALUES (34, ?, ?, ?, ?, ?)
'''
cursor.execute(insert_sql, row_33)
print("✓ Inserted duplicate 0701 row at ID 34")

conn.commit()

# Verify
print("\n✓ Final state:")
cursor.execute('SELECT ID, GroupCode FROM WatchGroups WHERE ID >= 33 AND ID <= 42 ORDER BY ID')
rows = cursor.fetchall()
for row in rows:
    print(f"  ID {row[0]}: {row[1]}")

cursor.execute('SELECT COUNT(*) FROM WatchGroups')
total = cursor.fetchone()[0]
print(f"\nTotal rows: {total}")

cursor.execute('SELECT ID FROM WatchGroups WHERE GroupCode = "0701" ORDER BY ID')
print(f"0701 rows at IDs: {[r[0] for r in cursor.fetchall()]}")

conn.close()
print("\n✓ Database updated successfully!")
