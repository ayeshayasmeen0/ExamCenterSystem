 📚 **Exam Center Management System**

A complete console-based application to manage students, exams, seats, and results for exam centers.

🚀 **Technologies Used**

| Technology | Purpose |
|------------|---------|
| 🟣 .NET 8 | Application Framework |
| 💻 C# | Programming Language |
| 🗄️ SQLite | File-based Database |
| 🐳 Docker | Containerization |
| 📦 Git & GitHub | Version Control |


✨ **Features**

### 👨‍🎓 Students
- Add, View, Update, Delete Students
- Reset Students (ID starts from 1)

📝 **Exams**
- Add, View, Update, Delete Exams
- Reset Exams

💺 **Seats**
- Department-wise allocation (CS, SE, IT, DS)
- 20 students per room (4×5 grid)
- Visual room layout
- View by department
- Update & Delete seats

📊 **Results**
- Add results with auto grade calculation
- Grades: A+, A, B+, B, C+, C, D, F
- Statistics (total, average, highest, lowest)
- Update & Delete results

🔐 **Admin**
- Secure login: `admin` / `1234`
- Password masking


🧠 **OOP Concepts**

| Concept | How Used |
|---------|----------|
| 🔒 Encapsulation | Private data with public methods |
| 👪 Inheritance | Student inherits from Person |
| 🎭 Polymorphism | GetGrade() changes with marks |
| 🫥 Abstraction | Services hide database logic |

**With Docker**
bash
docker build -t examcentersystem:latest .
docker run -it --rm examcentersystem:latest

🐳 **Docker Commands**
Command	What it does
docker build -t examcentersystem:latest .	Build image
docker run -it --rm examcentersystem:latest	Run container
docker ps	See running containers
docker images	See all images

📊 **Grade System**
Marks	Grade
90-100	A+
80-89	A
75-79	B+
70-74	B
60-69	C+
50-59	C
40-49	D
Below 40	F

📁 **Project Structure**
text
ExamCenterSystem/
├── 📂 Data/        # Database connection
├── 📂 Models/      # Student, Exam, Seat, Result classes
├── 📂 Services/    # Business logic
├── 📄 Program.cs   # Main menu
├── 🐳 Dockerfile   # Docker config
└── 📖 README.md    # This file

👩‍💻 **Author**
Ayesha Yasmim
🎓 BSCS | Semester 6A
📧 ayeshayasmen347@gmail.com
### Locally
```bash
dotnet run
