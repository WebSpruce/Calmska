# Calmska

AI‑assisted mood tracker with Pomodoro timer and Hygge‑inspired wellbeing tips, built with .NET MAUI and a .NET backend.

![Calmska preview](CALMSKA.gif)

[Online demo](https://appetize.io/app/b_2mxop2skrgrccf7akq7ruj2m4a)

> The mobile app is currently intended for Android.

---

## Overview

Calmska lets you log how you feel in natural language; an AI model turns your text into a single mood and stores it in your history.  
After at least five days of entries, the app can analyze your recent moods and suggest short, practical Hygge‑style tips tailored to your patterns.  
A built‑in Pomodoro timer with background notifications helps you structure focused work and breaks alongside mood tracking.

---

## Main features

- **AI mood entry** – type how you feel, get a single classified mood, and save it with one tap.
- **Mood history & analysis** – view past moods and trigger Hygge advice after enough recent entries.
- **Pomodoro timer** – configurable work/break sessions with a circular progress indicator and sound.
- **Account & sync** – email/password sign‑in backed by Firebase and a MongoDB‑based API.
- **Settings** – choose daily reminder time, toggle notifications, and manage basic preferences.

---

## Tech stack

- **Client:** .NET MAUI (Android‑first), MVVM with CommunityToolkit.Mvvm, CommunityToolkit.Maui, Plugin.Maui.Audio.
- **Backend API:** ASP.NET Core minimal APIs, MongoDB via MongoDB.EntityFrameworkCore, API versioning, OpenAPI/Scalar UI for docs.
- **Auth:** Firebase email/password authentication.
- **AI:** HTTP chat‑completion API with a configurable host/model and an AI “firewall” that screens both prompts and responses.
- Clean Architecture, Dependency Injection, AutoMapper, MediatR, xUnit, Fluent Assertions, Moq
---

## Run the backend

Requirements: .NET SDK with `net10.0` support, MongoDB, Firebase project, AI (openai format) provider key, AutoMapper license key.

1. Set configuration (environment variables):

  - `mongoDbUri`, `mongoDbName`
  - `calmska_firebaseApiKey`
  - `ai_api_key`, `ai_api_host`, `ai_api_model`
  - `automapper_key`

2. Start the API:

   ```bash
   cd Calmska.Api
   dotnet run

[🖌 View App UI/UX Design](https://www.figma.com/design/mdhVEHFrAAc71qLnXgYBFo/Calmska?node-id=0-1&t=RiXjXKAvAoGOzCzG-1) | 
[💿 View Database Schema](https://www.figma.com/design/KHtrSLFCdqJfANaMcqE7qa/Relational-Database-Diagram---Component-Kit-(Community)?node-id=3-728&t=izB1EdeXBzwRAZs7-1)