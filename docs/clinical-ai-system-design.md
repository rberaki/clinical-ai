# 1. Problem Definition

### Objective

Design and implement a backend clinical decision-support system that detects early signs of acute patient deterioration and generates explainable, auditable alerts to support clinical escalation.

### Clinical Motivation

Hospitalized and ICU patients can deteriorate rapidly due to sepsis, respiratory failure, hemorrhage, cardiac instability, or other acute conditions. Delayed recognition increases morbidity and mortality.

Rapid Response Teams (RRTs) exist to intervene early — but recognition is inconsistent and manual.

This system aims to:

- Continuously monitor patient physiological data
- Predict risk of deterioration in near-term horizons (e.g., 6–24 hours)
- Provide explainable alerts
- Integrate into clinical workflow (tasks/escalation)

### Non-Goals

This system:
- Does NOT replace clinicians
- Does NOT autonomously initiate treatment
- Does NOT function as a diagnostic system

It is a decision-support augmentation tool, not an autonomous medical agent.

---

## 2. Clinical Context

### Patient
An individual receiving care within the hospital system.

### Encounter
A period of care (e.g., hospital admission or ICU stay).

### ICU Stay
A high-acuity encounter where patients are continuously monitored.

### Observations
Time-stamped clinical measurements, including:
- Vital signs (HR, BP, RR, SpO₂, temperature)
- Lab results (lactate, WBC, creatinine, etc.)
- Device-derived signals (ventilator parameters)

### Rapid Response Team (RRT)
A specialized clinical team activated when a patient shows signs of acute deterioration.

### Escalation
Clinical action triggered when deterioration risk crosses a threshold (e.g., RRT activation, ICU transfer).

### Alert Fatigue
Excessive false or low-value alerts leading clinicians to ignore or override alerts.

Mitigation of alert fatigue is a key design constraint.

---

## 3. Clinical Event Definition (Target Outcome)

### Acute Deterioration Event

Defined as the earliest occurrence of:

1. Initiation of vasopressor therapy  
2. Initiation of invasive mechanical ventilation  
3. In-hospital death  

This composite endpoint represents clinically meaningful deterioration requiring escalation.

### Modeling Targets

The system will support two prediction paradigms:

1. **Short-term classification**
   - Predict probability of deterioration within next 6h / 12h / 24h

2. **Survival analysis**
   - Estimate time-to-deterioration from admission
   - Output risk curve / hazard trajectory

The time origin will initially be ICU admission.

---

## 4. System Architecture Overview

### Components

1. **Clinical API (.NET Core)**
   - Core domain logic
   - Workflow engine
   - Alert and task management
   - Audit logging
   - FHIR publishing

2. **PostgreSQL**
   - Domain storage
   - Append-only event store
   - Lineage ledger
   - Watermarks

3. **FHIR Server (HAPI FHIR)**
   - Interoperability layer
   - Stores Patient, Encounter, Observation resources
   - Allows standards-compliant representation

4. **Feature Pipeline (Python jobs)**
   - Incremental transformation of raw events
   - Produces model-ready features
   - Maintains watermarks

5. **ML Service (FastAPI)**
   - Hosts trained models
   - Exposes `/predict_risk`
   - Returns risk scores + optional survival outputs

6. **Alert Engine**
   - Threshold logic
   - Escalation rules
   - Alert deduplication

7. **Lineage Ledger**
   - Records feature runs
   - Records prediction runs
   - Records alert decisions
   - Enables full explainability

---

## 5. Mini-Ontology (Semantic Domain Layer)

Inspired by modern data platform ontology patterns.

### Core Objects

#### Patient
- id
- demographics
- relationships → Encounters

#### Encounter
- id
- patientId
- location
- admissionTime
- status
- relationships → Observations, Alerts

#### Observation
- id
- encounterId
- code (HR, BP, etc.)
- value
- timestamp
- source

#### FeatureRun
- id
- encounterId
- inputEventIds
- featureVersion
- windowStart
- windowEnd
- featureHash

#### PredictionRun
- id
- featureRunId
- modelVersion
- predictedRisk
- horizon
- survivalCurveSummary
- timestamp

#### Alert
- id
- encounterId
- predictionRunId
- riskScore
- threshold
- status (Active, Acknowledged, Escalated, Closed)
- createdAt

#### ClinicalTask
- id
- alertId
- taskType (RRT Activation, ICU Consult)
- assignedTo
- status

---

## 6. End-to-End Data Flow

### Step 1 — Clinical Event Ingestion

Observations are ingested as append-only `raw_event` records.

No updates. No deletes.

### Step 2 — Incremental Feature Computation

Feature pipeline:
- Reads new raw events using watermark
- Computes windowed features
- Writes FeatureRun
- Advances watermark

### Step 3 — Risk Prediction

Clinical API calls ML service:
- Sends feature vector
- Receives:
  - Risk probabilities
  - Survival metrics

PredictionRun is persisted.

### Step 4 — Alert Evaluation

Alert engine:
- Compares risk to threshold
- Applies suppression rules
- Creates Alert if criteria met

### Step 5 — Escalation Workflow

Alert may generate:
- ClinicalTask
- Escalation action

### Step 6 — Explainability

Full traceability available:
- Alert → PredictionRun → FeatureRun → raw_event IDs

---

## 7. Non-Functional Requirements

### 7.1 Traceability
Every alert must be explainable with:
- Input data used
- Model version
- Feature version
- Threshold logic

### 7.2 Reproducibility
Given:
- Same input events
- Same feature code version
- Same model version

System must reproduce same prediction.

### 7.3 Incremental Processing
- Append-only ingestion
- Watermark-based ETL
- Idempotent replays

### 7.4 Auditability
All:
- Alert state changes
- Task actions
- Overrides

Must be logged with timestamps and actor identity.

### 7.5 Alert Fatigue Control
- Cooldown windows
- Risk persistence checks
- Configurable thresholds

### 7.6 Monitoring
Track:
- Alert rate per 100 encounters
- Model drift indicators
- Data missingness patterns
- Distribution shifts

### 7.7 Safety Boundaries
- No automated treatment
- Alerts must require human acknowledgment
- System failures must fail safely (no silent suppression)

---

## 8. Technology Stack

- .NET Core (Clinical API)
- PostgreSQL
- HAPI FHIR
- Python (Feature pipeline + ML)
- FastAPI (Model serving)
- Docker (Local orchestration)

---

## 9. Future Extensions

- Multi-horizon risk modeling
- Time-dependent Cox model
- Random Survival Forest
- Model calibration layer
- Role-based access controls
- UI dashboard for clinicians
