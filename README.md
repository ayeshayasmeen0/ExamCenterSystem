# 🎓 Exam Center System

A console-based **Exam Center Management System** developed using **C# (.NET 8)** and **Object-Oriented Programming (OOP)** concepts. This project manages students, exams, seating, and results efficiently.

---

## 📌 Features

* 👨‍🎓 Student Registration & Management
* 📝 Exam Creation & Handling
* 💺 Seat Allocation System
* 📊 Result Calculation & Display
* 🔐 Admin Controls
* 🐳 Docker Support (Run Anywhere)

---

## 🛠️ Technologies Used

* C# (.NET 8)
* OOP (Encapsulation, Inheritance, Polymorphism, Abstraction)
* SQL Database (via DbConnection)
* Docker
* Visual Studio / VS Code

---

## 📂 Project Structure

```
ExamCenterSystem/
│
├── Data/
│   ├── DbConnection.cs
│   └── DbInitializer.cs
│
├── Models/
│   ├── Person.cs
│   ├── Student.cs
│   └── Exam.cs
│
├── Services/
│   ├── AdminService.cs
│   ├── ExamService.cs
│   ├── ResultService.cs
│   ├── SeatService.cs
│   └── StudentService.cs
│
├── Program.cs
└── Dockerfile
```

---

## 🧠 OOP Concepts Used

### 🔒 Encapsulation

Data is hidden inside classes using properties (e.g., Student, Exam).

### 🧬 Inheritance

`Student` class inherits from `Person`.

### 🔁 Polymorphism

Different services perform operations in their own way.

### 🎭 Abstraction

Service layer hides implementation details from the main program.

---

## ▶️ How to Run (Without Docker)

```bash
dotnet build
dotnet run
```

---

## 🐳 Run with Docker

### 1️⃣ Build Docker Image

```bash
docker build -t examcentersystem .
```

### 2️⃣ Run Container

```bash
docker run -it examcentersystem
```

---

## 📸 Sample Output

```
WELCOME TO EXAM CENTER SYSTEM
1. Register Student
2. Manage Exams
3. View Results
4. Exit
```

---

## 📈 Future Improvements

* GUI (Windows Forms / Web App)
* Online Exam Feature
* Authentication System
* Advanced Reporting

---

## 👩‍💻 Author

**Ayesha Yasmin**
BSCS – Semester 6A
Registration No: 232201023

---

## ⭐ Note

This project is developed for educational purposes to demonstrate **OOP concepts and system design** in C#.

---
