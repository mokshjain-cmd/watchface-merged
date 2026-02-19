import sqlite3
import os

db_path = r'D:\Downloads\WatchFace\jieli-middleware\temp\方形_240x284_普通_201#简约_My Watch Face_001_00\watch.db'

print(f"Connecting to database: {db_path}")
print(f"Database exists: {os.path.exists(db_path)}")

conn = sqlite3.connect(db_path)
cursor = conn.cursor()

# Check current row count
cursor.execute('SELECT COUNT(*) FROM WatchGroups')
total_rows = cursor.fetchone()[0]
print(f"\nTotal rows before: {total_rows}")

# Find the existing 0701 row
cursor.execute('SELECT * FROM WatchGroups WHERE GroupCode = "0701"')
existing_row = cursor.fetchone()
print(f"Existing 0701 row: {existing_row}")

if existing_row:
    # Get column names
    cursor.execute('PRAGMA table_info(WatchGroups)')
    columns = cursor.fetchall()
    print(f"\nTable columns: {[col[1] for col in columns]}")
    
    # Insert duplicate row (all same values except ID which will auto-increment)
    # Assuming structure: ID, WatchTypeId, GroupCode, Left, Top, ColorDesc
    if len(existing_row) >= 6:
        insert_sql = '''
        INSERT INTO WatchGroups (WatchTypeId, GroupCode, Left, Top, ColorDesc)
        VALUES (?, ?, ?, ?, ?)
        '''
        cursor.execute(insert_sql, (existing_row[1], existing_row[2], existing_row[3], existing_row[4], existing_row[5]))
        conn.commit()
        print(f"\n✓ Successfully inserted duplicate 0701 row")
    else:
        print(f"\nERROR: Unexpected row structure with {len(existing_row)} columns")

# Verify the duplicate was added
cursor.execute('SELECT * FROM WatchGroups WHERE GroupCode = "0701"')
rows_0701 = cursor.fetchall()
print(f"\nAll 0701 rows after insert:")
for row in rows_0701:
    print(f"  {row}")

# Check new total
cursor.execute('SELECT COUNT(*) FROM WatchGroups')
total_rows_after = cursor.fetchone()[0]
print(f"\nTotal rows after: {total_rows_after}")
print(f"Added {total_rows_after - total_rows} row(s)")

conn.close()
print("\n✓ Database updated successfully!")
