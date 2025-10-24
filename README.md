# ExpenseSplit

ExpenseSplit is a Blazor WebAssembly application built with .NET 8, designed to help users manage and split expenses among friends. The app provides a user-friendly interface for creating expenses, selecting participants, and choosing different split methods (equally, by exact amount, or by percentage).

## Features

-   **User Authentication:** Secure login and registration using ASP.NET Core authentication.
-   **Create Expenses:** Add new expenses, set descriptions, amounts, dates, and select participants.
-   **Split Methods:** Choose to split expenses equally, by exact amount, or by percentage.
-   **Manage Friends:** Add, view, and manage your friends for easy expense sharing.
-   **Track Debts:** View your debts and credits with friends.
-   **Local Storage Support:** Uses Blazored.LocalStorage for persistent client-side data.
-   **Responsive UI:** Modern, mobile-friendly design.

## Technologies Used

-   **Blazor WebAssembly (.NET 8)**
-   **ASP.NET Core Components**
-   **Blazored.LocalStorage**
-   **System.Net.Http.Json**
-   **Microsoft.Extensions.Http**

## Getting Started

### Prerequisites

-   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
-   [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)
-   Node.js (for frontend build tools, if needed)

### Installation

1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/roshan-maharjan/ASD-489-Project-Frontend.git](https://github.com/roshan-maharjan/ASD-489-Project-Frontend.git)
    cd ASD-489-Project-Frontend/ExpenseSplit
    ```

2.  **Restore NuGet packages:**
    ```bash
    dotnet restore
    ```

3.  **Build and run the project:**
    ```bash
    dotnet run
    ```

4.  **Open in browser:**
    Navigate to `https://localhost:5001` (or the port shown in the console).

## Usage

-   Register a new account or log in.
-   Add friends to your account.
-   Create a new expense, select participants, and choose how to split the expense.
-   View your dashboard to track debts and credits.

## Project Structure

-   `Pages/` - Blazor components for UI pages (Home, Register, Login, CreateExpense, Friends, etc.)
-   `Services/` - Service classes for authentication, user management, and API calls.
-   `Models/` - Data transfer objects and models.
-   `wwwroot/` - Static files and assets.
-   `ExpenseSplit.csproj` - Project configuration and dependencies.

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request.

## License

This project is licensed under the MIT License.

## Acknowledgements

-   [Blazored.LocalStorage](https://github.com/Blazored/LocalStorage)
-   [Microsoft ASP.NET Core](https://docs.microsoft.com/aspnet/core/blazor/)