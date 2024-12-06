# Operating-Systems-Project

# Mini-FAT File System

## Overview
The **Mini-FAT File System** is a simplified implementation of the FAT (File Allocation Table) structure using C#. It simulates a virtual disk with 1MB storage, supporting cluster-based data management, file allocation tracking via a FAT table, and metadata operations. This project demonstrates essential file system design concepts.

---

## Features
- **Virtual Disk Simulation**:
  - Represents a 1MB virtual disk with 1024 clusters, each 1024 bytes.
  - Includes superblock, FAT table, and content regions.

- **File Allocation Table (FAT)**:
  - Tracks cluster usage: `0` (free), `-1` (end of chain), and positive values (next cluster).
  - Stored in clusters 1-4.

- **Cluster Operations**:
  - Read and write 1024-byte data blocks efficiently.

- **Free Space Tracking**:
  - Calculate remaining space dynamically.

---

## How It Works
1. **Disk Initialization**:
   - Creates a virtual disk with superblock (unused), FAT table, and empty content clusters.
   
2. **File Allocation**:
   - Manages cluster allocation using a linked-list structure in the FAT table.

3. **Data Management**:
   - Supports reading and writing data blocks in 1024-byte clusters.

4. **Free Space Calculation**:
   - Dynamically calculates available storage.

---

## Code Highlights
### Key Methods
- **`CreateOrOpenDisk(string path)`**:
  Initializes or opens the virtual disk file.
- **`WriteCluster(byte[] cluster, int clusterIndex)`**:
  Writes 1024 bytes to a specific cluster.
- **`ReadCluster(int clusterIndex)`**:
  Reads 1024 bytes from a specific cluster.
- **`SaveFAT()` / `LoadFAT()`**:
  Saves and retrieves the FAT table from disk.
- **`GetFreeSpace()`**:
  Calculates available storage space.
- **`CloseDisk()`**:
  Saves the FAT table and closes the virtual disk file.

---

## Project Structure
1. **Superblock (Cluster 0)**:
   - Reserved for metadata (currently unused).

2. **FAT Table (Clusters 1-4)**:
   - Stores cluster allocation information.

3. **Content Region (Clusters 5-1023)**:
   - Holds file and directory data.

