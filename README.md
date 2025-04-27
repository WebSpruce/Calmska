# ✨ Calmska – Find Your Inner Balance

**<a href="https://github.com/WebSpruce/Calmska#main-features" >Main Features</a>** - **<a href="https://github.com/WebSpruce/Calmska#how-the-ai-mood-analyzer-works" >How the AI Mood Analyzer Works</a>** - **<a href="https://github.com/WebSpruce/Calmska#project-layout">Project Layout</a>**

<img src="https://github.com/WebSpruce/Calmska/blob/main/CALMSKA.gif?raw=true" height="400" alt="Holiday Calendar Screenshot">

**<a href="https://appetize.io/app/b_dk7gkpbjoov2e7gwt34val33ke" target=”_blank” style="font-size: 20px; color:#99aab5 ;">ONLINE DEMO</a>**

> [!IMPORTANT]
> The app is available on android ONLY (tested on Android 12)

(The app is currently hosted on a free server, so logging in might take a little longer.)

## 📖 Main Features
- **Mood Tracker with Smart Notifications & AI Emotion Analyzer**
- **Pomodoro Timer** with customizable work/break sounds
- **Hygge Tips** to inspire mindfulness and cozy living

---

## 🚀 Why You'll Love Calmska
- 📅 **Daily Mood Tracking** — Get reminders at your preferred time to check in with your emotions.
- 🧠 **AI-Powered Emotion Recognition** — Write a few sentences, and Calmska AI will instantly summarize your mood into a single feeling word.
- 📝 **Custom Mood Entries** — Quickly log your emotions with a tap.
- 🧘 **Hygge Advice** — After saving moods for 5 days, unlock personalized Scandinavian "Hygge" lifestyle tips to boost your well-being.
- 🔔 **Custom Notifications** — Easily enable/disable notifications and set the exact time you want to receive them.
- 📊 **Mood Analysis History** — Track how your emotions evolve over time.

---

## ✨ How the AI Mood Analyzer Works
1. Enter your current feelings or sentences.
2. LLM (Language Model) analyzes your input and distills it into one emotion.
3. Your mood is automatically saved in the database.
4. After logging moods for 5 days, Calmska’s second LLM suggests tailored **Hygge** tips to uplift your daily life.

---

### 🏗️ Project Layout
- MAUI
- API
  - <strong>Program.cs</strong>: Configures application endpoints, dependency injection, AutoMapper, and repository setup.
  - <strong>CalmskaDbContext.cs</strong>: Manages database initialization and table definitions.
  - <strong>Properties</strong>: Includes launchSettings.json for environment-specific configurations.
  - <strong>DTO</strong>: Contains Data Transfer Objects (DTOs) for structured communication between API and clients.
  - <strong>Helpers</strong>: Utility classes and methods such as:
	- <strong>HashPassword</strong> for secure password management.
	- <strong>MapperProfiles</strong> for object-to-object mappings.
	- <strong>Pagination</strong> logic for API responses.
  - <strong>Interfaces</strong>: Defines contracts for dependency injection and ensures flexibility in implementation.
  - <strong>Repository</strong>: repositories with core database operations, implementing the Repository Pattern.
- Models:
  - Models: 
	- <strong>Entity Models</strong>: Defines the structure of database tables and entities.
	- <strong>MongoDbSettings</strong>: Configuration for MongoDB connection and settings.

---

### 🛠️ Tools
- <strong>MongoDB</strong>: database for scalable and flexible data storage.
- <strong>REST API</strong>: Clean and efficient APIs for client-server communication.
- <strong>MVVM</strong>: Model-View-ViewModel for robust application architecture.
- <strong>Dependency Injection</strong>: Achieves loosely coupled and testable code.
- <strong>Repository Pattern</strong>: Encapsulates data access logic and promotes cleaner architecture.
- <strong>DTO (Data Transfer Objects)</strong>: Streamlines communication between layers.
- <strong>AutoMapper</strong>: Simplifies object mapping and transformation.
- <strong>Pagination</strong>: Efficient handling of large datasets in API responses.

---

[🖌 View App UI/UX Design](https://www.figma.com/design/mdhVEHFrAAc71qLnXgYBFo/Calmska?node-id=0-1&t=RiXjXKAvAoGOzCzG-1) 
[💿 View Database Schema](https://www.figma.com/design/KHtrSLFCdqJfANaMcqE7qa/Relational-Database-Diagram---Component-Kit-(Community)?node-id=3-728&t=izB1EdeXBzwRAZs7-1)

---

##### 📝 Upcoming Features
- Mood analises history list