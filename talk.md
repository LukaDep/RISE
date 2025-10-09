# TALK

```mermaid
graph TD

%% =========================
%% Projecten
%% =========================

Client --> Shared
Server --> Client
Server --> Persistence
Server --> Services
Server --> Shared
Services --> Domain
Services --> Persistence
Services --> Shared
Persistence --> Domain
```
