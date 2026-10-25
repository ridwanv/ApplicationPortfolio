# Agent Output File Structure

## 📁 Directory Organization

```
.claude/
├── workflows/
│   ├── {feature-name}-{timestamp}/           # Each feature gets its own workflow folder
│   │   ├── 01-planning/
│   │   │   ├── trend-research.json           # trend-researcher output
│   │   │   ├── ux-research.json              # ux-researcher output
│   │   │   ├── sprint-plan.json              # sprint-prioritizer output
│   │   │   └── planning-summary.md           # Human-readable summary
│   │   ├── 02-design/
│   │   │   ├── ui-design.json                # ui-designer output
│   │   │   ├── backend-architecture.json     # backend-architect output
│   │   │   ├── whimsy-enhancements.json      # whimsy-injector output
│   │   │   ├── wireframes/                   # UI wireframes and mockups
│   │   │   └── design-summary.md             # Human-readable summary
│   │   ├── 03-implementation/
│   │   │   ├── backend-implementation.json   # blazor-clean-architect output
│   │   │   ├── frontend-implementation.json  # frontend-developer output
│   │   │   ├── test-results.json             # test-writer-fixer output
│   │   │   ├── code-changes.md               # List of files created/modified
│   │   │   └── implementation-summary.md     # Human-readable summary
│   │   ├── 04-delivery/
│   │   │   ├── deployment.json               # project-shipper output
│   │   │   ├── experiments.json              # experiment-tracker output
│   │   │   ├── analytics.json                # analytics-reporter output
│   │   │   └── delivery-summary.md           # Human-readable summary
│   │   ├── workflow-state.json               # Current workflow status
│   │   └── feature-manifest.json             # Complete feature documentation
│   └── active-workflow.json                  # Points to current active workflow
├── templates/
│   ├── agent-input-template.json             # Standard input format
│   ├── agent-output-template.json            # Standard output format
│   └── workflow-template.json                # Workflow initialization template
└── config/
    ├── agent-contracts.json                  # Agent I/O contracts (existing)
    ├── workflow-settings.json                # Workflow configuration
    └── project-context.json                  # Blazor project context
```

## 📋 File Naming Convention

### Workflow Folders
```
{feature-name}-{yyyy-mm-dd-hhmm}/
```
**Examples:**
- `ai-document-classification-2025-08-21-1430/`
- `user-analytics-dashboard-2025-08-21-0915/`
- `mobile-responsive-ui-2025-08-22-1100/`

### Agent Output Files
```
{agent-name}-{phase}.json
```
**Examples:**
- `trend-research.json`
- `ui-design.json`
- `backend-implementation.json`

## 📄 Specific Agent Output Locations

### Phase 1: Planning Agents
```bash
.claude/workflows/ai-document-classification-2025-08-21-1430/01-planning/
├── trend-research.json           # Market validation, trending keywords
├── ux-research.json              # User pain points, journey maps  
├── sprint-plan.json              # Prioritized features, timeline
└── planning-summary.md           # Executive summary of planning phase
```

### Phase 2: Design Agents  
```bash
.claude/workflows/ai-document-classification-2025-08-21-1430/02-design/
├── ui-design.json                # Component specs, wireframes
├── backend-architecture.json     # Entity models, CQRS commands
├── whimsy-enhancements.json      # Delight moments, animations
├── wireframes/
│   ├── upload-flow.png
│   ├── classification-results.png
│   └── batch-processing.png
└── design-summary.md             # Design decisions and rationale
```

### Phase 3: Implementation Agents
```bash
.claude/workflows/ai-document-classification-2025-08-21-1430/03-implementation/
├── backend-implementation.json   # Domain/Application layer files
├── frontend-implementation.json  # Blazor components, UI logic
├── test-results.json             # Test coverage, results
├── code-changes.md               # Git-style diff summary
└── implementation-summary.md     # Technical implementation notes
```

### Phase 4: Delivery Agents
```bash
.claude/workflows/ai-document-classification-2025-08-21-1430/04-delivery/
├── deployment.json               # Release plan, feature flags
├── experiments.json              # A/B test config, metrics
├── analytics.json                # Dashboard setup, KPIs
└── delivery-summary.md           # Launch checklist and metrics
```

## 🔄 Workflow State Management

### workflow-state.json
```json
{
  "feature_name": "ai-document-classification",
  "workflow_id": "ai-document-classification-2025-08-21-1430",
  "current_phase": "02-design",
  "current_agent": "backend-architect",
  "status": "in_progress",
  "started_at": "2025-08-21T14:30:00Z",
  "agents_completed": [
    "trend-researcher",
    "ux-researcher", 
    "sprint-prioritizer",
    "ui-designer"
  ],
  "agents_remaining": [
    "backend-architect",
    "whimsy-injector",
    "blazor-clean-architect",
    "frontend-developer",
    "test-writer-fixer",
    "project-shipper",
    "experiment-tracker",
    "analytics-reporter"
  ],
  "context_chain": [
    "01-planning/trend-research.json",
    "01-planning/ux-research.json", 
    "01-planning/sprint-plan.json",
    "02-design/ui-design.json"
  ]
}
```

### feature-manifest.json
```json
{
  "feature_name": "ai-document-classification",
  "description": "AI-powered estate document classification",
  "workflow_id": "ai-document-classification-2025-08-21-1430",
  "final_deliverables": {
    "domain_entities": [
      "src/Domain/Entities/ClassifiedDocument.cs",
      "src/Domain/Entities/DocumentType.cs"
    ],
    "application_features": [
      "src/Application/Features/Documents/"
    ],
    "ui_components": [
      "src/Server.UI/Pages/Documents/DocumentClassification.razor"
    ],
    "tests": [
      "tests/Application.UnitTests/Features/Documents/"
    ]
  },
  "success_metrics": {
    "classification_accuracy": "95%",
    "processing_time": "< 30 seconds",
    "test_coverage": "94%"
  },
  "completed_at": "2025-08-21T18:45:00Z"
}
```

## 🔍 Context Retrieval

### How Agents Access Previous Output
```bash
# Current agent reads context from previous phases
current_context = {
  "planning_output": load_json("01-planning/sprint-plan.json"),
  "design_output": load_json("02-design/ui-design.json"),
  "previous_agent": load_json("02-design/backend-architecture.json")
}
```

### Cross-Phase References
```json
{
  "input_sources": [
    "01-planning/sprint-plan.json:features",
    "02-design/ui-design.json:component_specifications"  
  ],
  "dependencies": [
    "backend-architecture.json:domain_entities",
    "ui-design.json:component_list"
  ]
}
```

## 🛠️ Implementation Commands

### Initialize New Workflow
```bash
# Creates workflow folder structure
claude-workflow init "ai-document-classification"
# Creates: .claude/workflows/ai-document-classification-2025-08-21-1430/
```

### Save Agent Output
```bash
# Agent saves to designated location
claude-workflow save-output \
  --agent="trend-researcher" \
  --phase="planning" \
  --output="market_validation_results.json"
```

### Load Context for Next Agent
```bash
# Next agent loads all relevant context
claude-workflow load-context \
  --agent="ux-researcher" \
  --sources="01-planning/trend-research.json"
```

### Archive Completed Workflow
```bash
# Move completed workflow to archive
claude-workflow archive "ai-document-classification-2025-08-21-1430"
# Moves to: .claude/archives/
```

## 📊 Benefits of This Structure

1. **Traceability**: Complete audit trail of all decisions
2. **Resumability**: Can pause and resume workflows at any point
3. **Reusability**: Templates can be reused for similar features
4. **Debugging**: Easy to identify where workflow issues occurred
5. **Documentation**: Human-readable summaries at each phase
6. **Parallel Workflows**: Multiple features can be developed simultaneously

## 🔄 Integration with Git

### .gitignore Additions
```bash
# Add to .gitignore
.claude/workflows/*/
!.claude/workflows/README.md
.claude/config/project-context.json
```

### Optional Commits
```bash
# Optionally commit major milestones
git add .claude/workflows/feature-name/04-delivery/feature-manifest.json
git commit -m "Complete AI document classification feature

- Market validated with 8/10 demand score
- 94% test coverage achieved  
- Deployed with A/B testing enabled"
```

This structure ensures every agent knows exactly where to save their output and where to find the context they need from previous agents.