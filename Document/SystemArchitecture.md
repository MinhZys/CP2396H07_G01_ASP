# System Architecture & Frontend Structure

## 1. Frontend Layout Structure
The application uses ASP.NET Core MVC Layouts to maintain a consistent look and feel across different sections.

### Public Section (Guest/Student)
*   **Layout File**: `Views/Shared/_Layout.cshtml`
*   **Header Location**: Inside `Views/Shared/_Layout.cshtml` (Lines ~57-152).
    *   Contains: Logo, Main Navigation (Home, Courses, Mentors...), Search Bar, User Profile Dropdown, **Language Toggle**.
*   **Usage**: Applied to Home, Courses, Mentors, Contact, and Profile pages.
*   **Key Scripts**:
    *   `system-notifications.js`: Handles global notifications.
    *   `i18next`: Handles frontend translation.
    *   `chat.js` (via `_ChatWidget` partial): Handles real-time chat.

### Admin Section (Administrator)
*   **Layout File**: `Views/Admin/Shared/_AdminLayout.cshtml`
*   **Header Location**: Inside `Views/Admin/Shared/_AdminLayout.cshtml` (Lines ~84-105).
    *   Contains: Admin Dashboard Title, **Language Toggle**, User Welcome Message, Logout.
*   **Sidebar**: Defined in the same file (Lines ~110-197).
*   **Usage**: Applied to all pages under the `Admin` area (Dashboard, Users, Courses Management...).

### Instructor Section (Instructor)
*   **Layout File**: `Views/Instructor/Shared/_InstructorLayout.cshtml`
*   **Usage**: Applied to pages under the `Instructor` area (My Classes, Grading...).

## 2. Notification System
The application uses a centralized mechanism to display success/error messages (Flash Messages) without repetitive code in every view.

### Workflow
1.  **Backend (Controller)**:
    *   Set `TempData["Success"] = "Message...";` or `TempData["Error"] = "Message...";`.
2.  **Layout (View)**:
    *   Reads `TempData` values and passes them to a JavaScript function.
    *   Code:
        ```javascript
        SystemNotification.showMessages(
            '@Html.Raw(TempData["Success"])', 
            '@Html.Raw(TempData["Error"])'
        );
        ```
3.  **Frontend (JavaScript)**:
    *   File: `wwwroot/js/system-notifications.js`
    *   Function: `showMessages(successMsg, errorMsg)`
    *   Library: **SweetAlert2** (`Swal.fire(...)`) is used to show the actual popup.

## 3. Localization (i18n)
*   **Library**: `i18next` (Client-side translation).
*   **Data Source**:
    *   English: `wwwroot/locales/en/translation.json`
    *   Vietnamese: `wwwroot/locales/vi/translation.json`
*   **Mechanism**:
    *   `_Layout.cshtml` initializes `i18next` and detects user language.
    *   Elements with `data-i18n="key"` are automatically translated.
    *   Language is persisted in Cookies/LocalStorage.

## 4. Real-time Chat
*   **Backend**: `Hubs/ChatHub.cs` (SignalR Hub).
*   **Frontend**: `wwwroot/js/chat.js`.
*   **UI Component**: `Views/Shared/_ChatWidget.cshtml` (Floating widget).
*   **Storage**: Messages are saved to `ChatMessages` table in SQL Server.
