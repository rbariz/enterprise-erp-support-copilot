# Enterprise ERP Support Copilot

Plateforme d’assistance intelligente destinée aux équipes support ERP.

Enterprise ERP Support Copilot combine intelligence artificielle, recherche vectorielle, capitalisation d’incidents et analyse assistée par LLM afin d’accélérer la résolution des tickets de support et la transmission de la connaissance opérationnelle.

---

# Vision

Les équipes support ERP consacrent une part importante de leur temps à :

* Rechercher dans la documentation interne
* Analyser les incidents déjà résolus
* Identifier les causes racines probables
* Produire des recommandations de résolution
* Capitaliser les connaissances acquises

L’objectif de ce projet est de fournir un copilote intelligent capable d’assister les analystes support dans leurs investigations quotidiennes.

---

## Screenshots

### Dashboard

![Dashboard](docs/images/dashboard.png)

### AI Ticket Analysis

![Analysis](docs/images/ticket-analysis.png)

### Knowledge Context & Similar Incidents

![Knowledge](docs/images/knowledge-context.png)

### Ticket Timeline

![Timeline](docs/images/timeline.png)

# Fonctionnalités principales

## Gestion des tickets

* Création et suivi des tickets
* Classification par catégorie
* Gestion des statuts
* Gestion des niveaux de sévérité
* Historique des analyses

## Analyse IA

Pour chaque ticket :

* Résumé automatique
* Détection des causes probables
* Recommandations d’actions
* Évaluation des risques
* Score de confiance

## Base de connaissances

* Gestion des articles de connaissance
* Recherche sémantique
* Indexation vectorielle
* Explicabilité des résultats

## Capitalisation des incidents

* Transformation d’analyses validées en incidents réutilisables
* Historique des résolutions
* Recherche d’incidents similaires
* Réutilisation des bonnes pratiques

## Activité et traçabilité

* Timeline des tickets
* Historique des analyses
* Journal d’activité
* Feedback utilisateur

---

# Pipeline RAG

Le moteur RAG (Retrieval Augmented Generation) constitue le cœur du système.

```text
Ticket
↓
Génération d'embedding
↓
Recherche vectorielle pgvector
↓
Recherche d'articles pertinents
↓
Recherche d'incidents similaires
↓
Construction du contexte RAG
↓
Analyse IA
↓
Recommandations
```

---

# Recherche Hybride

Le moteur de recherche combine deux approches :

## Recherche vectorielle

Basée sur :

* Ollama
* nomic-embed-text
* PostgreSQL pgvector

Permet la recherche sémantique sur :

* Workflow
* Notifications
* Approbations
* Interfaces
* Erreurs applicatives

## Recherche par mots-clés

Permet d'améliorer les résultats pour les éléments très techniques :

* ORA-xxxxx
* APP-FND-xxxxx
* FRM-xxxxx
* Concurrent Requests
* Noms de programmes Oracle

## Score hybride

Le classement final combine :

```text
Score Vectoriel
+
Score Mots-Clés
=
Score Final
```

---

# Explicabilité du RAG

Chaque résultat affiche :

* Score de similarité
* Concepts détectés
* Tags correspondants
* Sources utilisées

Exemple :

```text
Workflow Notification Troubleshooting

Similarity 80%

Matched Terms:
- workflow
- notification
- mailer
```

---

# Architecture Technique

```text
Support Ticket
↓
Embedding Generation
↓
pgvector Search
↓
Knowledge Retrieval
↓
Similar Incident Retrieval
↓
RAG Context Builder
↓
LLM Analysis
↓
AI Recommendations
```

---

# Stack Technique

## Backend

* .NET 9
* ASP.NET Core Minimal APIs
* Entity Framework Core
* PostgreSQL
* pgvector

## Intelligence Artificielle

### Analyse

* Groq
* llama-3.1-8b-instant

### Embeddings

* Ollama
* nomic-embed-text

## Frontend

* Blazor
* Bootstrap

---

# Fonctionnalités actuellement disponibles

## Support

* Gestion des tickets
* Gestion des incidents résolus
* Historique des analyses
* Timeline des tickets
* Activité récente

## IA

* Analyse automatique
* Recherche vectorielle
* Recherche hybride
* Context Builder
* Similarity Scoring
* RAG Explainability

## Base de connaissances

* Articles de connaissance
* Embeddings
* Recherche sémantique
* Recherche hybride

---

# Cas d'usage ciblés

Le projet cible principalement :

* Oracle E-Business Suite (EBS)
* Oracle Forms
* Oracle Workflow
* Oracle Purchasing
* Oracle Payables
* Oracle General Ledger
* Oracle Inventory

Exemples :

* Workflow Notification Not Sent
* PO Approval Workflow Stuck
* Concurrent Program Failure
* Supplier Interface Errors
* OAF Page Exceptions
* ORA-00001 Unique Constraint Violations

---

# Roadmap

## Phase 1 — MVP

* Gestion des tickets
* Analyse IA
* Base de connaissances
* Incidents résolus
* Recherche vectorielle

## Phase 2 — Intelligence Support

* Recherche hybride avancée
* Explainability enrichie
* Prompt Preview
* Feedback Learning Loop
* Source Ranking

## Phase 3 — Enterprise

* Multi-ERP
* Multi-tenant
* Gestion des équipes
* Reporting avancé
* Export PDF

---

# Statut du projet

Projet en développement actif.

Version actuelle : MVP fonctionnel avec pipeline RAG complet, recherche vectorielle pgvector, recherche hybride et analyse assistée par IA.
