# NetWatch - Distributed Hardware Telemetry System (C2)

**NetWatch** is a high-performance Command & Control (C2) dashboard designed to monitor real-time hardware telemetry from distributed agents. 
The system demonstrates a decoupled architecture using **gRPC** for high-throughput data streaming and **.NET 8 WPF** for visualization.

![Dashboard Preview](dashboard_preview.png)

## 🏗 Architecture & Tech Stack

The solution implements **Clean Architecture** principles and enforces strict separation of concerns:

### 1. The Agent (Python) - `TelemetryService.py`
A lightweight background service acting as the field agent.
* **Low-Level Monitoring:** Utilizes `psutil` to access kernel-level metrics (CPU interrupts, Memory paging, Network I/O).
* **Binary Streaming:** Data is serialized via **Protobuf** and streamed over HTTP/2 using **gRPC**, ensuring minimal network overhead compared to REST/JSON.
* **Real-World Simulation:** Calculates real-time network delta (KB/s) rather than static counters.

### 2. The Dashboard (C# .NET 8) - `NetWatch.Client`
A WPF-based visualization layer implementing the **MVVM** design pattern.
* **Services Layer:** `GrpcDeviceService` implements `IDeviceService`, decoupling communication logic from the UI.
* **Reactive UI:** Uses `ObservableCollection` and `INotifyPropertyChanged` for thread-safe UI updates on the Dispatcher.
* **Performance:** Renders hardware-accelerated charts (SkiaSharp) capable of handling high-frequency updates (10Hz).
* **Configuration:** No hard-coded values; all thresholds and endpoints are managed via `appsettings.json`.

## 🚀 Key Features
* **Interoperability:** Seamless communication between Python (Linux/Windows Agent) and .NET (Windows Client).
* **Resilience:** Auto-reconnection logic and visual status indicators (Green/Red connectivity states).
* **Data Analysis:** In-memory LINQ aggregations for real-time average calculation.
* **Testability:** Includes a separate xUnit testing project (`NetWatch.Tests`) verifying core logic and extension methods.
* **Scalability:** Designed with Dependency Injection (DI) readiness for future expansion.

## 🛠️ Getting Started

### Prerequisites
* .NET 8 SDK
* Python 3.9+
* `pip install -r requirements.txt`

### 1. Start the Telemetry Agent
```bash
python TelemetryService.py
* "The agent will start broadcasting on port 50051."

### 2. Launch the Dashboard
Open a new terminal:

Bash

cd NetWatch.Client
dotnet run

### 3. Run Unit Tests
To verify system logic and architecture integrity:

Bash

dotnet test
