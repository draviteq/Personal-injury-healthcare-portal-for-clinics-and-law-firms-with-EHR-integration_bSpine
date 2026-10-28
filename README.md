# Patient Privacy Compliance Web Application

## 📄 Application Overview

This application is built to help clinics in the United States securely manage patient information while ensuring compliance with HIPAA regulations. It supports encrypted data storage, secure access, and controlled sharing of patient records.

## ⚙️ Tech Stack

- **Backend:**
  - .NET 8.0
  - Entity Framework Core (EF Core)
  - LINQ
  - SQL Server (with encrypted columns)
  - Tebra SOAP Service Integration (for fetching patient data)
  - Stripe Payment Gateway (for capturing payments)
  - Backend validation using .NET Data Annotations

- **Frontend:**
  - HTML, jQuery
  - Bootstrap (including Bootstrap form validation)
  - SpRuha Theme for UI/UX design

## 🔐 Security & Compliance

- All sensitive data in the database is encrypted using **EF Core Column Encryption**.
- Ensures **HIPAA compliance** for handling patient data.
- Access to patient records is strictly protected and governed by U.S. privacy laws.
- Integration with Tebra SOAP Service enables secure fetching of real-time patient information.

## 💳 Payment Handling

- Integrated with **Stripe Payment Gateway** to capture and process patient payments securely.

## 🏥 Purpose

To assist clinics in meeting federal privacy standards by securely managing patient data, reducing the risk of unauthorized disclosures, and ensuring compliance with healthcare regulations (HIPAA).

## 📦 Configuration & Setup Notes

- Store SOAP service credentials, database keys, and Stripe keys securely (e.g., AWS Secrets Manager or environment variables).
- Use HTTPS in all environments to ensure encrypted transmission.
- Encryption keys should not be stored in source code or public repositories.