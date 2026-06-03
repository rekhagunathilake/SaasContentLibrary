# saas-content-library-api

A B2B SaaS content library API for asset-management platforms — versioned 
content blocks with a compliance-driven approval workflow, in .NET 10.

## What this demonstrates

- Clean Architecture (Domain → Application → Infrastructure → Api)
- DDD: aggregate roots, value objects, domain events, strongly-typed IDs
- CQRS with MediatR — commands, validators, pipeline behaviors  
- `Result<T>` pattern for domain failures (no exceptions for business rules)
- EF Core 10 with Postgres, strongly-typed ID conversions, value-object conversions
- (planned) OpenTelemetry tracing across HTTP → MediatR → EF Core

## Architecture

\`\`\`mermaid
classDiagram
    direction LR

    class Entity~TId~ {
        <<abstract>>
        +TId Id
    }

    class AggregateRoot~TId~ {
        <<abstract>>
        +IReadOnlyList~IDomainEvent~ DomainEvents
        +ClearDomainEvents()
    }

    class IDomainEvent {
        <<interface>>
        +DateTime OccurredOnUtc
    }

    class ContentBlock {
        <<sealed>>
        +TenantId TenantId
        +BlockType BlockType
        +Locale Locale
        +string Name
        +BlockStatus Status
        +DateTime CreatedAtUtc
        +DateTime? ArchivedAtUtc
        +IReadOnlyList~ContentVersion~ Versions
        +Create()$ Result~ContentBlock~
        +AddDraftVersion() Result~ContentVersionId~
        +SubmitForReview() Result
        +ApproveVersion() Result
        +Archive() Result
    }

    class ContentVersion {
        <<sealed>>
        +int VersionNumber
        +ContentBody Body
        +VersionStatus Status
        +string AuthoredBy
        +DateTime AuthoredAtUtc
        +string? ApprovedBy
        +DateTime? ApprovedAtUtc
    }

    class Locale {
        <<record>>
        +string Code
        +Create()$ Result~Locale~
    }

    class ContentBody {
        <<record>>
        +string Value
        +Create()$ Result~ContentBody~
    }

    class BlockType {
        <<enum>>
        Disclaimer
        ManagerBio
        FundDescription
        MarketingCopy
    }

    class BlockStatus {
        <<enum>>
        Active
        Archived
    }

    class VersionStatus {
        <<enum>>
        Draft
        InReview
        Approved
        Superseded
    }

    Entity <|-- AggregateRoot
    AggregateRoot <|-- ContentBlock
    Entity <|-- ContentVersion

    ContentBlock "1" *-- "1..*" ContentVersion : owns
    ContentBlock --> Locale : has
    ContentBlock --> BlockType : classified as
    ContentBlock --> BlockStatus : in state
    ContentVersion --> ContentBody : holds
    ContentVersion --> VersionStatus : in state

    AggregateRoot ..> IDomainEvent : raises
\`\`\`

## Approval workflow

\`\`\`mermaid
flowchart TD
    Start([Asset manager creates new content block])
    --> Create["CreateContentBlock<br/>(name, type, locale, body, author)"]
    Create --> V1[Block: Active<br/>Version 1: Draft]

    V1 --> Decision1{Ready<br/>to publish?}

    Decision1 -->|Yes| Submit1[SubmitForReview]
    Decision1 -->|No, edit further| AddDraft[AddDraftVersion<br/>new body]

    AddDraft --> V2[New version: Draft]
    V2 --> Submit2[SubmitForReview]

    Submit1 --> InReview1[Version: InReview]
    Submit2 --> InReview2[Version: InReview]

    InReview1 --> Review{Approver<br/>reviews}
    InReview2 --> Review

    Review -->|Approve| Approve[ApproveVersion]
    Review -->|Needs rework| AddDraft

    Approve --> CheckPrev{Was there<br/>a prior approved<br/>version?}

    CheckPrev -->|Yes| Supersede[Prior version<br/>auto-demoted to<br/>Superseded]
    CheckPrev -->|No| Final
    Supersede --> Final

    Final[Version: Approved<br/>canonical text]

    Final --> EndDecision{Block still<br/>relevant?}

    EndDecision -->|Yes, keep using| Done([Live content<br/>consumed by document generation])
    EndDecision -->|No, retire| Archive[ArchiveBlock]

    Archive --> Archived([Block: Archived<br/>no further changes])
\`\`\`

## Getting started

\`\`\`bash
docker compose up -d postgres
dotnet tool restore
dotnet ef database update --project SaasContentLibrary.Infrastructure --startup-project SaasContentLibrary.Api
dotnet run --project SaasContentLibrary.Api
\`\`\`

## Status

Currently in progress. Domain, Application, and Infrastructure layers complete; 
Api endpoints and OpenTelemetry instrumentation in progress.

## Known workarounds

The Api project explicitly references `Microsoft.CodeAnalysis.Workspaces.Common 5.0.0` 
to work around an EF Core 10 preview tooling issue. To be removed when EF Core 10 
patches deploy Roslyn dependencies correctly.