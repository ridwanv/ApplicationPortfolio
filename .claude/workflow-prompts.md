# Agent Workflow Prompts

## 🚀 Starting a New Feature Workflow

### Prompt Template
```
I want to build a new feature using the agent workflow. Here's my idea:

**Feature**: [Description of the feature]
**Goal**: [Business objective or user problem to solve]
**Priority**: [High/Medium/Low]
**Timeline**: [6-day sprint or other]

Please start the agent workflow beginning with trend-researcher to validate this concept, then continue through the full pipeline: UX → UI → Backend → Implementation → Launch.

Save all agent outputs to the workflow directory structure we established.
```

### Example Prompts

#### 1. AI-Powered Feature
```
I want to build a new feature using the agent workflow. Here's my idea:

**Feature**: AI-powered inheritance calculator that analyzes estate documents and provides distribution recommendations based on legal requirements and family dynamics.

**Goal**: Help estate attorneys provide faster, more accurate inheritance planning while reducing manual calculation errors by 90%.

**Priority**: High - this could be a major differentiator for the platform

**Timeline**: 6-day sprint

Please start the agent workflow beginning with trend-researcher to validate this concept, then continue through the full pipeline.
```

#### 2. Social/Viral Feature  
```
I want to build a new feature using the agent workflow. Here's my idea:

**Feature**: "Estate Story" - a feature that generates beautiful, shareable timeline visualizations of family asset history that people can post on social media.

**Goal**: Drive viral growth by making estate planning feel personal and shareable, targeting younger demographics who want to understand their family legacy.

**Priority**: Medium - could drive significant user acquisition

**Timeline**: 6-day sprint

Please start the agent workflow with trend-researcher to assess viral potential and market demand.
```

#### 3. Productivity Enhancement
```
I want to build a new feature using the agent workflow. Here's my idea:

**Feature**: Smart document templates that auto-populate based on client data and previous similar cases, with AI suggestions for common clauses.

**Goal**: Reduce document preparation time by 70% and ensure consistency across all legal documents.

**Priority**: High - direct productivity impact for attorneys

**Timeline**: 6-day sprint

Please initiate the full agent workflow starting with market research.
```

## 🔄 Continuing Existing Workflows

### Resume Workflow Prompt
```
I have an existing workflow at `.claude/workflows/[workflow-name]/` that was paused at the [phase] phase. 

The last completed agent was [agent-name]. Please:
1. Load the workflow state and previous agent outputs
2. Continue with the next agent: [next-agent-name]
3. Ensure context is properly passed from previous phases

Current workflow: [workflow-name]
```

### Example Resume Prompt
```
I have an existing workflow at `.claude/workflows/ai-inheritance-calculator-2025-08-21-1430/` that was paused at the design phase.

The last completed agent was ui-designer. Please:
1. Load the workflow state and previous agent outputs  
2. Continue with the next agent: backend-architect
3. Ensure context is properly passed from the planning and ui-design phases

Current workflow: ai-inheritance-calculator-2025-08-21-1430
```

## 🔧 Specific Agent Prompts

### Individual Agent Execution
```
Please run the [agent-name] agent for the feature: [feature-description]

Context from previous agents:
- [Previous agent outputs or manual context]

Requirements:
- [Specific requirements for this agent]

Save the output to the appropriate workflow directory following our established structure.
```

### Example Individual Agent Prompts

#### UI Designer Only
```
Please run the ui-designer agent for the feature: Smart Document Templates

Context from previous agents:
- Market research shows 85% demand for template automation
- Users need: drag-drop template builder, clause library, client data integration
- Sprint plan prioritizes: template editor, auto-population, preview functionality

Requirements:
- Use MudBlazor components
- Design for both desktop and tablet
- Include accessibility considerations
- Focus on attorney workflow efficiency

Save the output to the appropriate workflow directory.
```

#### Backend Architect Only
```
Please run the backend-architect agent for the feature: Estate Story Visualization

Context from previous agents:
- UI design specifies: timeline component, photo upload, sharing features
- User research shows need for: family tree integration, asset timeline, privacy controls

Requirements:
- Design Clean Architecture domain models
- Plan CQRS commands and queries
- Consider integration with existing estate/document entities
- Include social sharing API contracts

Save the output to the appropriate workflow directory.
```

## 🎯 Specialized Workflow Prompts

### Rapid Prototype Mode
```
I need a rapid prototype for: [feature-idea]

Please use the rapid-prototyper agent to create a working MVP, then follow up with:
1. ux-researcher for user validation
2. whimsy-injector for delight moments  
3. test-writer-fixer for basic testing

Timeline: 2-3 days maximum
Focus: Speed over perfection, validate concept quickly
```

### Mobile-First Feature
```
I want to build a mobile-first feature: [feature-description]

Please run the workflow with special focus on:
- mobile-app-builder for implementation
- ux-researcher should prioritize mobile user journeys
- ui-designer should design mobile-first, desktop-second
- Performance optimization for mobile devices

Feature: [description]
Target: Mobile users primarily, web secondary
```

### Experimental Feature with A/B Testing
```
I want to build an experimental feature that we'll A/B test: [feature-description]

Please run the full workflow with emphasis on:
- experiment-tracker should design comprehensive A/B test setup
- Multiple UI variants from ui-designer
- Clear success metrics from sprint-prioritizer
- Analytics dashboard from analytics-reporter

Feature: [description]
Hypothesis: [what we're testing]
Success metrics: [how we'll measure success]
```

## 🚨 Emergency/Hotfix Prompts

### Critical Bug Fix Workflow
```
We have a critical issue that needs immediate attention: [issue-description]

Please run a streamlined workflow:
1. Skip trend-research and ux-research  
2. Go directly to backend-architect or blazor-clean-architect
3. Prioritize test-writer-fixer for comprehensive testing
4. Fast-track through project-shipper for deployment

Issue: [description]
Impact: [business/user impact]
Timeline: [hours/days]
```

### Security Issue Workflow
```
Security issue detected: [security-issue]

Please run security-focused workflow:
1. Use backend-architect with security-first design
2. Include legal-compliance-checker for regulatory requirements
3. Comprehensive testing with test-writer-fixer
4. Careful deployment with project-shipper (staged rollout)

Issue: [security concern]
Compliance requirements: [any relevant regulations]
```

## 📊 Analysis and Optimization Prompts

### Performance Optimization
```
I need to optimize performance for: [feature/page]

Please run performance-focused workflow:
1. performance-benchmarker to identify bottlenecks
2. backend-architect for server-side optimizations
3. frontend-developer for client-side improvements
4. test-writer-fixer to validate improvements

Current performance issues: [specific problems]
Target improvements: [specific goals]
```

### User Experience Review
```
Please review and improve the UX for: [existing-feature]

Run user-focused workflow:
1. ux-researcher to identify current pain points
2. analytics-reporter to analyze usage data
3. ui-designer to propose improvements
4. whimsy-injector to add delight moments
5. test-writer-fixer to validate changes

Current feature: [feature-name]
Known issues: [user complaints/feedback]
```

## 🎨 Creative/Innovation Prompts

### Trend-Based Feature Development
```
I saw this trending on [platform]: [trend-description]

Please explore if we can build something around this:
1. trend-researcher to validate opportunity and competition
2. ux-researcher to understand how it applies to our users
3. Creative ideation for how this fits our estate planning platform
4. Full workflow if the concept validates

Trend: [description]
Platform: [TikTok/Twitter/LinkedIn/etc]
Potential application: [initial thoughts]
```

### Viral Feature Brainstorming
```
I want to create a feature that could go viral and drive user acquisition.

Please run a creativity-focused workflow:
1. trend-researcher to identify viral mechanics and current trends
2. ux-researcher to find intersection of trends with our user needs  
3. ui-designer to create shareable, visually appealing designs
4. whimsy-injector to maximize delight and shareability
5. tiktok-strategist to plan social media amplification

Goal: Drive organic user growth through shareability
Target audience: [demographics]
Platform focus: [social platforms]
```

## 💡 Pro Tips for Effective Prompts

### Be Specific About Context
```
✅ Good: "Building for estate attorneys who process 50+ documents daily"
❌ Bad: "Building for lawyers"
```

### Include Success Criteria
```
✅ Good: "Reduce document processing time by 70%, maintain 99% accuracy"
❌ Bad: "Make it faster"
```

### Specify Constraints
```
✅ Good: "Must work on tablets, integrate with existing MudBlazor design system"
❌ Bad: "Make it look good"
```

### Reference Previous Work
```
✅ Good: "Similar to the document classification feature we built, but for contracts"
❌ Bad: "Build something like that other thing"
```

These prompts will help you effectively leverage the agent workflow system for any type of feature development, from rapid prototypes to complex enterprise features.