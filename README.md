# Up-To-Date (UTD) - News Aggregator Web Application

![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-green)
![SQL Server](https://img.shields.io/badge/SQL%20Server-Express-lightgrey)

UTD is a secure, role-based news aggregator web application built with ASP.NET Core MVC. It allows users to stay updated with the latest news while supporting Authors, Readers, and Contributors with distinct functionalities.

---

## 📌 Features
- **Role-Based Access Control**:
  - **Authors**: Create, publish, and delete news articles.
  - **Readers**: Browse, search, and read news; submit suggestions; upload/download articles.
  - **Contributors**: Edit existing news articles.
- **Security Measures**: Protection against SQL injection, XSS, DoS attacks, and secure password hashing.
- **MVC Architecture**: Clean separation of concerns with Model-View-Controller design.
- **Real-Time Updates**: News feed dynamically reflects changes.
- **Search Functionality**: Keyword-based search for news articles.

---

## 🛠 Technologies Used
- **Backend**: ASP.NET Core 8.0, C#, Entity Framework Core
- **Frontend**: Razor Pages, HTML/CSS, JavaScript
- **Database**: Microsoft SQL Server
- **Testing**: xUnit, Moq
- **Security**: ASP.NET Core Identity, HTTPS, Role-Based Authorization

---

## ⚙ Prerequisites
- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) (with ASP.NET workload) or VS Code
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Git](https://git-scm.com/) (optional)

---

## 🚀 Installation & Setup

1. **Clone the Repository**:
   ```bash
   git clone https://code.umd.edu/sumi0309/ENPM680Fall2024Project-sumi0309.git
   ```

2. **Restore Dependencies**:
   ```bash
   dotnet restore
   ```

3. **Configure the Database**:
   - Update the connection string in `appsettings.json`:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DB;Trusted_Connection=True;"
     }
     ```
   - Apply migrations:
     ```bash
     dotnet ef database update
     ```

4. **Run the Application**:
   - Build the solution (`Ctrl+Shift+B` in Visual Studio).
   - Launch via IIS Express (`Ctrl+F5`). The app will run at `https://localhost:44369`.
   
