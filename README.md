![Operating Systems Banner](https://github.com/abdallasamir04/Operating-Systems-Project/raw/main/OS/osbanner.png)


# 🖥️ Operating Systems Project : Mini-FAT File System


![Version](https://img.shields.io/badge/version-1.0.0-green.svg?style=for-the-badge&logo=github&logoColor=white)
![C#](https://img.shields.io/badge/Language-C%23-0078d4.svg?style=for-the-badge&logo=csharp&logoColor=white)

The **Mini-FAT File System** is a simplified implementation of the FAT (File Allocation Table) structure using **C#**.  
It simulates a virtual disk with **1MB storage**, supporting cluster-based data management, file allocation tracking via a FAT table, and metadata operations.  
This project demonstrates essential file system design concepts and was developed by **_Abdalla Samir_** as part of the **Operating Systems** course at **Assiut National University**.

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

## 📜 Commands

The Mini-FAT File System supports the following commands for managing files and directories:

| Command     | Description                                  | Example                     |
|-------------|----------------------------------------------|-----------------------------|
| `cd`        | Change directory                             | `cd k:\folder`              |
| `dir`       | List directory contents                      | `dir k:\folder`             |
| `copy`      | Copy files/directories                       | `copy file.txt k:\backup`   |
| `import`    | Import files from physical disk              | `import C:\data.txt`        |
| `export`    | Export files to physical disk                | `export data.txt D:\`       |
| `md`        | Create a directory                           | `md new_folder`             |
| `rd`        | Remove a directory                           | `rd old_folder`             |
| `cls`       | Clear screen                                 | `cls`                       |
| `help`      | Show command help                            | `help copy`                 |
| `type`      | Display file content                         | `type file.txt`             |
| `rename`    | Rename a file or directory                   | `rename old.txt new.txt`    |
| `del`       | Delete a file                                | `del file.txt`              |

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


## 🙏 Acknowledgments


    Prof. Khaled Fathy - Project supervision.

    Eng. Khaled Gamal - Technical guidance.

    
## 📧 Contact

Let's connect! Feel free to reach out or follow me on these platforms:  

[![X (Twitter)](https://img.shields.io/badge/X-black.svg?style=for-the-badge&logo=X&logoColor=white)](https://x.com/abdallasamir04)  
[![Facebook](https://img.shields.io/badge/Facebook-1877F2?style=for-the-badge&logo=facebook&logoColor=white)](https://www.facebook.com/abdallasamir04/)  
[![Telegram](https://img.shields.io/badge/Telegram-2CA5E0?style=for-the-badge&logo=telegram&logoColor=white)](https://t.me/abdallasamir04)  
[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/abdalla-mahmoud-9264242b6/)  
[![Email](https://img.shields.io/badge/Email-D14836?style=for-the-badge&logo=gmail&logoColor=white)](mailto:samirovic707@gmail.com)  
[![GitHub](https://img.shields.io/badge/GitHub-%23121011.svg?style=for-the-badge&logo=github&logoColor=white)](https://github.com/abdallasamir04)  
---
**Abdalla Mahmoud Samir**  
**Faculty of Computers and Artificial Intelligence**  
**Assiut National University**  
**Operating Systems Course (CS321) - Third Level**
