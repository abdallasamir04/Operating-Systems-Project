# 🖥️ Operating Systems Project: Mini-FAT File System

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![Version](https://img.shields.io/badge/version-1.0.0-green.svg)
![C#](https://img.shields.io/badge/Language-C%23-purple.svg)

The **Mini-FAT File System** is a simplified implementation of the FAT (File Allocation Table) structure using **C#**. It simulates a virtual disk with **1MB storage**, supporting cluster-based data management, file allocation tracking via a FAT table, and metadata operations. This project demonstrates essential file system design concepts and was developed as part of the **Operating Systems** course at **Assiut National University**.

---

## 🚀 Features

### Virtual Disk Simulation
- Represents a **1MB virtual disk** with **1024 clusters**, each **1024 bytes**.
- Includes **superblock**, **FAT table**, and **content regions**.

### File Allocation Table (FAT)
- Tracks cluster usage:
  - `0`: Free cluster.
  - `-1`: End of cluster chain.
  - Positive values: Next cluster in the chain.
- Stored in **clusters 1-4**.

### Cluster Operations
- Efficiently **read** and **write** 1024-byte data blocks.

### Free Space Tracking
- Dynamically calculates **remaining storage space**.

---

## 🛠️ How It Works

### Disk Initialization
- Creates a virtual disk with:
  - **Superblock** (unused).
  - **FAT table**.
  - Empty **content clusters**.

### File Allocation
- Manages cluster allocation using a **linked-list structure** in the FAT table.

### Data Management
- Supports **reading** and **writing** data blocks in 1024-byte clusters.

### Free Space Calculation
- Dynamically calculates **available storage**.

---

## 💻 Code Highlights

### Key Methods
| Method                          | Description                                      |
|---------------------------------|--------------------------------------------------|
| `CreateOrOpenDisk(string path)` | Initializes or opens the virtual disk file.      |
| `WriteCluster(byte[], int)`     | Writes 1024 bytes to a specific cluster.         |
| `ReadCluster(int)`              | Reads 1024 bytes from a specific cluster.        |
| `SaveFAT()` / `LoadFAT()`       | Saves and retrieves the FAT table from disk.     |
| `GetFreeSpace()`                | Calculates available storage space.              |
| `CloseDisk()`                   | Saves the FAT table and closes the virtual disk. |

---

## 📂 Project Structure

### Superblock (Cluster 0)
- Reserved for metadata (currently unused).

### FAT Table (Clusters 1-4)
- Stores cluster allocation information.

### Content Region (Clusters 5-1023)
- Holds file and directory data.

---

## ⚙️ Installation

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/abdallasamir04/os-virtual-disk.git
   cd os-virtual-disk

## 🤝 Contributing

Contributions are welcome! Please:

    Fork the repository.

    Create a feature branch (git checkout -b feature/your-feature).

    Commit changes (git commit -m 'Add feature').

    Push to the branch (git push origin feature/your-feature).

    Open a Pull Request.

## 🙏 Acknowledgments


    Prof. Khaled Fathy - Project supervision.

    Eng. Khaled Gamal - Technical guidance.

    
## 📧 Contact

Abdalla Samir

    Email: samirovic707@gmail.com

    GitHub: @abdallasamir04

    Twitter: @abdallasamir04

Faculty of Computers and Artificial Intelligence
Assiut National University
Operating Systems Course - Third Level
