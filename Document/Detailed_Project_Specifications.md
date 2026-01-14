# Detailed System Specifications

## 1. AUTHENTICATION & USERS

### 1.1 Authentication
#### 1.1.1 Login
| Field | Description |
|-------|-------------|
| **Purpose** | Allow users (Admin, Instructor, Student) to access the system. |
| **Inputs** | Email, Password |
| **Outputs** | Redirect to the appropriate Dashboard based on Role. Show error if credentials are invalid. |

#### 1.1.2 Register
| Field | Description |
|-------|-------------|
| **Purpose** | Allow visitors or new students to create an account. |
| **Inputs** | Full Name, Email, Password, Confirm Password |
| **Outputs** | Account created successfully. Redirect to Login page. |

#### 1.1.3 Logout
| Field | Description |
|-------|-------------|
| **Purpose** | End the current user's session. |
| **Inputs** | Click "Sign out" button |
| **Outputs** | Destroy session, redirect to Home page (Public). |

### 1.2 Manage Users (Admin)
#### 1.2.1 Search & View Users
| Field | Description |
|-------|-------------|
| **Purpose** | View list of users and search by name or email. |
| **Inputs** | Search Keyword (Name/Email), Filter by Status (Active/Inactive) |
| **Outputs** | User list table (Name, Email, Role, Status). |

#### 1.2.2 Create User
| Field | Description |
|-------|-------------|
| **Purpose** | Admin manually creates a new account (e.g., create Instructor account). |
| **Inputs** | Full Name, Email, Password, Role (Admin/Teacher/Student) |
| **Outputs** | New user added to database. |

#### 1.2.3 Edit User
| Field | Description |
|-------|-------------|
| **Purpose** | Update personal information or privileges of a user. |
| **Inputs** | Full Name, Email, Role, Status |
| **Outputs** | User information updated. |

#### 1.2.4 Toggle Status (Enable/Disable)
| Field | Description |
|-------|-------------|
| **Purpose** | Lock or unlock user account without deleting. |
| **Inputs** | User ID, Confirm Action |
| **Outputs** | Status IsActive toggles from True -> False or vice-versa. |

#### 1.2.5 Delete User
| Field | Description |
|-------|-------------|
| **Purpose** | Permanently remove user account from system. |
| **Inputs** | User ID, Confirm Delete |
| **Outputs** | User removed from database (with confirmation dialog). |

---

## 2. ACADEMIC MANAGEMENT (Admin)

### 2.1 Manage Categories
#### 2.1.1 List & Search Categories
| Field | Description |
|-------|-------------|
| **Purpose** | View all course categories. |
| **Inputs** | Search Keyword |
| **Outputs** | List of categories with Name, Description, Display Order. |

#### 2.1.2 Create/Edit Category
| Field | Description |
|-------|-------------|
| **Purpose** | Add or modify a course category. |
| **Inputs** | Name, Description, Display Order, IsActive |
| **Outputs** | Category saved. |

#### 2.1.3 Delete Category
| Field | Description |
|-------|-------------|
| **Purpose** | Remove a category. |
| **Inputs** | Category ID, Confirmation |
| **Outputs** | Category deleted (prevent if courses exist). |

### 2.2 Manage Certificates
#### 2.2.1 Search & List Certificates
| Field | Description |
|-------|-------------|
| **Purpose** | View and search for existing certificates. |
| **Inputs** | Search Keyword (Certificate Name) |
| **Outputs** | List of certificates matching criteria. |

#### 2.2.2 Create Certificate
| Field | Description |
|-------|-------------|
| **Purpose** | Add a new certificate type to the system. |
| **Inputs** | Name, Description, Validity Period |
| **Outputs** | New certificate saved. |

#### 2.2.3 Edit Certificate
| Field | Description |
|-------|-------------|
| **Purpose** | Modify details of an existing certificate. |
| **Inputs** | Name, Description, Validity Period |
| **Outputs** | Certificate details updated. |

#### 2.2.4 Delete Certificate
| Field | Description |
|-------|-------------|
| **Purpose** | Remove a certificate from the system. |
| **Inputs** | Certificate ID, Confirmation |
| **Outputs** | Certificate deleted (validate no active courses attached). |

### 2.3 Manage Subjects
#### 2.3.1 Search & View Subjects
| Field | Description |
|-------|-------------|
| **Purpose** | View list of existing subjects. |
| **Inputs** | Search Keyword (Subject Name) |
| **Outputs** | Subject list (Image, ID, Name, StudyTime). |

#### 2.3.2 Create Subject
| Field | Description |
|-------|-------------|
| **Purpose** | Add a new subject to the curriculum. |
| **Inputs** | Subject ID (Unique), Name, Study Time (hours), Description, Image File |
| **Outputs** | New subject saved. |

#### 2.3.3 Edit Subject
| Field | Description |
|-------|-------------|
| **Purpose** | Edit subject information. |
| **Inputs** | Name, Study Time, Description, New Image (Subject ID immutable) |
| **Outputs** | Subject information updated. |

#### 2.3.4 Delete Subject
| Field | Description |
|-------|-------------|
| **Purpose** | Remove a subject from the system. |
| **Inputs** | Subject ID, Confirm Delete |
| **Outputs** | Subject deleted. |

### 2.4 Manage Courses
#### 2.4.1 List, Search & Filter Courses
| Field | Description |
|-------|-------------|
| **Purpose** | View all courses with advanced filtering. |
| **Inputs** | Search by Name, Filter by Category, Filter by Level |
| **Outputs** | Course list with images, status badges, and details. |

#### 2.4.2 Create Course
| Field | Description |
|-------|-------------|
| **Purpose** | Create a course composed of subjects. |
| **Inputs** | Title, Description, Duration, Tuition Fee, Level, **Category**, **Certificate**, Image, **Subject Selection** |
| **Outputs** | New course created. |

#### 2.4.3 Edit Course
| Field | Description |
|-------|-------------|
| **Purpose** | Update course details. |
| **Inputs** | Course details, new image, modify subject list |
| **Outputs** | Course updated. |

### 2.5 Manage Classes
#### 2.5.1 Create Class
| Field | Description |
|-------|-------------|
| **Purpose** | Open a new class for a specific course. |
| **Inputs** | Course Selection, Class Name, Room, Schedule |
| **Outputs** | New class created. |

#### 2.5.2 Assign Teacher
| Field | Description |
|-------|-------------|
| **Purpose** | Assign a teacher to be in charge of a class. |
| **Inputs** | Class ID, Teacher Selection |
| **Outputs** | Teacher assigned to class. |

---

## 3. ADMISSIONS (Admin & Guest)

### 3.1 Entrance Exams
#### 3.1.1 Register for Exam (Guest)
| Field | Description |
|-------|-------------|
| **Purpose** | Guests register for entrance examination. |
| **Inputs** | Full Name, Phone, Email, Exam Date Selection, Payment Receipt Code |
| **Outputs** | Registration status set to Pending. |

#### 3.1.2 Manage Registrations (Admin)
| Field | Description |
|-------|-------------|
| **Purpose** | View and process exam registrations. |
| **Inputs** | Registration ID, Approve/Reject Action |
| **Outputs** | If Approved: Send notification & schedule. If Rejected: Cancel registration. |

### 3.2 Question Bank
#### 3.2.1 Create Question
| Field | Description |
|-------|-------------|
| **Purpose** | Add questions to the multiple-choice question bank. |
| **Inputs** | Question Content, Option A/B/C/D, Correct Answer |
| **Outputs** | New question saved. |

#### 3.2.2 List Questions
| Field | Description |
|-------|-------------|
| **Purpose** | View existing list of questions. |
| **Inputs** | Filter by Subject |
| **Outputs** | List of questions displayed. |

---

## 4. INSTRUCTOR PORTAL

### 4.1 Manage Lessons
#### 4.1.1 Create Lesson
| Field | Description |
|-------|-------------|
| **Purpose** | Create lesson content for a class session. |
| **Inputs** | Class ID, Lesson Title, Content/Description, Date |
| **Outputs** | New lesson visible to students. |

#### 4.1.2 Edit Lesson
| Field | Description |
|-------|-------------|
| **Purpose** | Edit lesson content. |
| **Inputs** | Title, Content |
| **Outputs** | Lesson content updated. |

### 4.2 Manage Materials
#### 4.2.1 Upload Material
| Field | Description |
|-------|-------------|
| **Purpose** | Upload learning materials (Slides, Sample Code). |
| **Inputs** | Select File, Title, Description, Class ID |
| **Outputs** | File saved on server, link displayed for class. |

#### 4.2.2 Delete Material
| Field | Description |
|-------|-------------|
| **Purpose** | Remove old/incorrect materials. |
| **Inputs** | Material ID, Confirm |
| **Outputs** | File deleted from system. |

### 4.3 Grading
#### 4.3.1 Update Grade
| Field | Description |
|-------|-------------|
| **Purpose** | Input exam/assignment grades for students. |
| **Inputs** | Student ID, Exam/Assignment ID, Score |
| **Outputs** | Grades saved to gradebook. |

---

## 5. CONTENT MANAGEMENT (CMS - Admin)

### 5.1 FAQ Management
#### 5.1.1 Create FAQ
| Field | Description |
|-------|-------------|
| **Purpose** | Add a new frequently asked question. |
| **Inputs** | Question, Answer |
| **Outputs** | FAQ displayed on Public page. |

#### 5.1.2 Edit/Delete FAQ
| Field | Description |
|-------|-------------|
| **Purpose** | Edit or delete an FAQ. |
| **Inputs** | FAQ ID, New Content |
| **Outputs** | FAQ list updated. |

### 5.2 Manage Centers
#### 5.2.1 Create/Edit Center
| Field | Description |
|-------|-------------|
| **Purpose** | Manage center location details. |
| **Inputs** | Name, Address, Phone, Map URL |
| **Outputs** | Contact info updated. |
