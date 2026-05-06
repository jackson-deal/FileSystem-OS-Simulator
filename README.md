# File System Simulator (CS3502 Project 3)

This application is a functional **File System Simulator** built using **C#** and **Windows Forms**. It serves as a practical demonstration of core Operating System concepts, specifically focusing on how low-level file operations are managed and presented to the user.

## Key Concepts Demonstrated
*   **File Handles & Management:** Simulates the creation, opening, and closing of file streams within a controlled environment.
*   **Directory Navigation:** Implements a hierarchical structure for browsing local directories.
*   **POSIX-Style Error Handling:** Maps standard .NET exceptions to traditional POSIX error codes for a more authentic OS simulation experience.

---

## Build & Run Instructions

To build and run this project locally, follow these steps:

### 1. Environment Setup
*   Ensure you have **Visual Studio (2019 or later)** installed.
*   Make sure the **.NET desktop development** workload is included in your installation.

### 2. Create a New Project
1.  Open Visual Studio and select **Create a new project**.
2.  Choose either **Windows Forms App (.NET Framework)** or **Windows Forms App**.
3.  **Important:** Name the project `CS3502_P3_FileSystem_JacksonDeal` to ensure the namespaces align with the provided source code.

### 3. Import Source Files
1.  In the Solution Explorer, **delete** the default `Form1.cs` and `Program.cs` files created by the template.
2.  Right-click your project, select **Add > Existing Item**, and import the following provided files:
    *   `Form1.cs`
    *   `Form1.Designer.cs`
    *   `Program.cs`

### 4. Add Required References
This project utilizes `Microsoft.VisualBasic` to handle input prompts:
1.  Right-click **References** (or **Dependencies**) in the Solution Explorer.
2.  Select **Add Reference**.
3.  Search for `Microsoft.VisualBasic`, check the box, and click **OK**.

### 5. Build and Run
1.  Press **F5** or click the **Start** button in Visual Studio.
2.  Upon launch, the application will initialize and point to your system's **Documents** folder by default.

---

**Jackson Deal**  
Kennesaw State University | Computer Science
