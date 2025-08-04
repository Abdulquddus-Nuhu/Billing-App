# Subscription Billing & Notification System

A complete .NET 7+ Web API implementing subscription management with auto-renewal, wallet deduction, and background notifications.

## 🚀 Features

- JWT Authentication (User and Admin roles)
- Subscription plans: Basic (monthly/30 days), Standard (quarterly/90 days), Premium (monthly/30 days)
- Subscribe, upgrade/downgrade, cancel
- Wallet simulation with balance deduction, initial user balance is 20,000
- Auto-renewal on expiry if user specifies auto-renewal at point of subscription
- Email reminders 3 days before renewal (simulated via logs)
- Admin dashboard: view all subscriptions and revenue summary
- Background service runs every hour to process renewals and send reminders (simulated via logs)
- SQLite database — **zero setup required**


## ▶️ How to Run (Zero Setup!)

✅ This project uses **SQLite**, so no SQL Server or connection strings needed!

1. **Clone the repo**
   git clone https://github.com/Abdulquddus-Nuhu/Billing-App.git,
   
   cd Billing-App
   
3. **Restore and run**
  dotnet restore
  dotnet build
  dotnet run --project src/BillingApp/BillingApp.csproj

4. **Open Swagger UI**
   After running, go to: _http://localhost:5155/swagger/index.html_ or _https://localhost:7152/swagger/index.html_

3. **Default Users**
Admin Role:
    Email:- demo@gmail.com
    Password:- DefaultPassword@1234
,

User Role:
    Email:- fakeuser@gmail.com
    Password:- DefaultPassword@1234


📂 Project Structure
/src
  /BillingApp                → Controllers, Program.cs
  /BillingApp.Application    → DTOs, Interfaces
  /BillingApp.Domain         → Entities, Enums
  /BillingApp.Infrastructure → EF, Services, Background
