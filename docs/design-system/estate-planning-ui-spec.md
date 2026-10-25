# Estate Planning Application - UI/UX Design System

## Table of Contents
1. [Design Philosophy](#design-philosophy)
2. [Design System Foundation](#design-system-foundation)
3. [Component Library](#component-library)
4. [Page Specifications](#page-specifications)
5. [User Flows](#user-flows)
6. [Implementation Guidelines](#implementation-guidelines)

---

## Design Philosophy

### Core Principles
- **Trust First**: Every design decision should reinforce credibility and security
- **Simplicity Over Complexity**: Estate planning is inherently complex - our UI should simplify, not add complexity
- **Guided Experience**: Users need clear direction through unfamiliar legal processes
- **Professional Yet Approachable**: Balance legal authority with user-friendly accessibility
- **Transparent Value**: Clear pricing and value propositions throughout

### Target Audience Considerations
- Age range: 25-65+ (multi-generational)
- Varying levels of tech comfort
- High anxiety around legal/financial decisions
- Need for reassurance and guidance
- Mobile-first behavior for younger users

---

## Design System Foundation

### Color Palette

#### Primary Colors
```css
/* Trust Blue - Primary brand color for CTAs and key elements */
--primary-blue: #2563EB;
--primary-blue-hover: #1D4ED8;
--primary-blue-light: #DBEAFE;

/* Success Green - Completed states, positive actions */
--success-green: #059669;
--success-green-light: #D1FAE5;

/* Warning Amber - Attention, incomplete states */
--warning-amber: #D97706;
--warning-amber-light: #FEF3C7;

/* Error Red - Errors, critical actions */
--error-red: #DC2626;
--error-red-light: #FEE2E2;
```

#### Neutral Palette
```css
/* Gray Scale for text, backgrounds, borders */
--gray-50: #F9FAFB;   /* Light backgrounds */
--gray-100: #F3F4F6;  /* Card backgrounds */
--gray-200: #E5E7EB;  /* Borders, dividers */
--gray-300: #D1D5DB;  /* Disabled states */
--gray-400: #9CA3AF;  /* Placeholders */
--gray-500: #6B7280;  /* Secondary text */
--gray-600: #4B5563;  /* Body text */
--gray-700: #374151;  /* Headings */
--gray-800: #1F2937;  /* Primary text */
--gray-900: #111827;  /* High emphasis text */
```

#### Accent Colors
```css
/* Professional accents for visual interest */
--accent-gold: #F59E0B;        /* Premium features */
--accent-gold-light: #FEF3C7;
--accent-purple: #7C3AED;      /* Advanced features */
--accent-purple-light: #EDE9FE;
```

### Typography System

#### Font Stack
```css
/* Primary font for headings and UI */
font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;

/* Secondary font for body text (better readability) */
font-family: 'Source Sans Pro', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
```

#### Type Scale
```css
/* Display - Hero headlines */
.text-display {
  font-size: 3rem;      /* 48px */
  line-height: 1.1;     /* 52.8px */
  font-weight: 700;
  letter-spacing: -0.025em;
}

/* H1 - Page titles */
.text-h1 {
  font-size: 2.25rem;   /* 36px */
  line-height: 1.2;     /* 43.2px */
  font-weight: 600;
  letter-spacing: -0.025em;
}

/* H2 - Section headers */
.text-h2 {
  font-size: 1.875rem;  /* 30px */
  line-height: 1.3;     /* 39px */
  font-weight: 600;
}

/* H3 - Subsection headers */
.text-h3 {
  font-size: 1.5rem;    /* 24px */
  line-height: 1.4;     /* 33.6px */
  font-weight: 600;
}

/* H4 - Card titles */
.text-h4 {
  font-size: 1.25rem;   /* 20px */
  line-height: 1.4;     /* 28px */
  font-weight: 600;
}

/* Body Large - Important body text */
.text-body-lg {
  font-size: 1.125rem;  /* 18px */
  line-height: 1.6;     /* 28.8px */
  font-weight: 400;
}

/* Body - Default text */
.text-body {
  font-size: 1rem;      /* 16px */
  line-height: 1.6;     /* 25.6px */
  font-weight: 400;
}

/* Body Small - Secondary text */
.text-body-sm {
  font-size: 0.875rem;  /* 14px */
  line-height: 1.5;     /* 21px */
  font-weight: 400;
}

/* Caption - Labels, captions */
.text-caption {
  font-size: 0.75rem;   /* 12px */
  line-height: 1.4;     /* 16.8px */
  font-weight: 500;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}
```

### Spacing System
```css
/* Base unit: 4px */
--space-1: 0.25rem;    /* 4px */
--space-2: 0.5rem;     /* 8px */
--space-3: 0.75rem;    /* 12px */
--space-4: 1rem;       /* 16px */
--space-5: 1.25rem;    /* 20px */
--space-6: 1.5rem;     /* 24px */
--space-8: 2rem;       /* 32px */
--space-10: 2.5rem;    /* 40px */
--space-12: 3rem;      /* 48px */
--space-16: 4rem;      /* 64px */
--space-20: 5rem;      /* 80px */
--space-24: 6rem;      /* 96px */
```

### Border Radius System
```css
--radius-sm: 0.25rem;   /* 4px - Small elements */
--radius-md: 0.5rem;    /* 8px - Default */
--radius-lg: 0.75rem;   /* 12px - Cards */
--radius-xl: 1rem;      /* 16px - Prominent cards */
--radius-full: 9999px;  /* Pills, circular elements */
```

### Shadow System
```css
/* Subtle elevation for cards */
--shadow-sm: 0 1px 2px 0 rgba(0, 0, 0, 0.05);

/* Default card elevation */
--shadow-md: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);

/* Prominent elements */
--shadow-lg: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);

/* Modal overlays */
--shadow-xl: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
```

---

## Component Library

### Button Components

#### Primary Button
```css
.btn-primary {
  background: var(--primary-blue);
  color: white;
  padding: 0.75rem 1.5rem;
  border-radius: var(--radius-md);
  font-weight: 600;
  font-size: 1rem;
  border: none;
  cursor: pointer;
  transition: all 0.2s ease;
  box-shadow: var(--shadow-sm);
}

.btn-primary:hover {
  background: var(--primary-blue-hover);
  box-shadow: var(--shadow-md);
  transform: translateY(-1px);
}

.btn-primary:active {
  transform: translateY(0);
  box-shadow: var(--shadow-sm);
}
```

#### Secondary Button
```css
.btn-secondary {
  background: white;
  color: var(--primary-blue);
  border: 2px solid var(--primary-blue);
  padding: 0.75rem 1.5rem;
  border-radius: var(--radius-md);
  font-weight: 600;
  font-size: 1rem;
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn-secondary:hover {
  background: var(--primary-blue-light);
}
```

#### Ghost Button
```css
.btn-ghost {
  background: transparent;
  color: var(--gray-600);
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: var(--radius-md);
  font-weight: 500;
  font-size: 1rem;
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn-ghost:hover {
  background: var(--gray-100);
  color: var(--gray-800);
}
```

### Card Components

#### Document Type Card
```css
.document-card {
  background: white;
  border: 2px solid var(--gray-200);
  border-radius: var(--radius-lg);
  padding: 2rem;
  box-shadow: var(--shadow-sm);
  transition: all 0.3s ease;
  cursor: pointer;
  position: relative;
  overflow: hidden;
}

.document-card:hover {
  border-color: var(--primary-blue);
  box-shadow: var(--shadow-lg);
  transform: translateY(-2px);
}

.document-card.selected {
  border-color: var(--primary-blue);
  background: var(--primary-blue-light);
}

.document-card-header {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.document-card-icon {
  width: 3rem;
  height: 3rem;
  background: var(--primary-blue-light);
  border-radius: var(--radius-lg);
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--primary-blue);
}

.document-card-title {
  font-size: 1.25rem;
  font-weight: 600;
  color: var(--gray-800);
}

.document-card-price {
  font-size: 1.875rem;
  font-weight: 700;
  color: var(--primary-blue);
  margin-bottom: 0.5rem;
}

.document-card-description {
  color: var(--gray-600);
  line-height: 1.6;
  margin-bottom: 1.5rem;
}

.document-card-features {
  list-style: none;
  padding: 0;
  margin: 0;
}

.document-card-feature {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 0.75rem;
  color: var(--gray-600);
  font-size: 0.875rem;
}

.document-card-feature-icon {
  color: var(--success-green);
  font-size: 1rem;
}
```

#### Progress Card
```css
.progress-card {
  background: white;
  border: 1px solid var(--gray-200);
  border-radius: var(--radius-lg);
  padding: 1.5rem;
  box-shadow: var(--shadow-sm);
}

.progress-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.progress-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: var(--gray-800);
}

.progress-percentage {
  font-size: 0.875rem;
  font-weight: 600;
  color: var(--primary-blue);
}

.progress-bar {
  width: 100%;
  height: 0.5rem;
  background: var(--gray-200);
  border-radius: var(--radius-full);
  overflow: hidden;
  margin-bottom: 1rem;
}

.progress-fill {
  height: 100%;
  background: linear-gradient(90deg, var(--primary-blue), var(--primary-blue-hover));
  border-radius: var(--radius-full);
  transition: width 0.3s ease;
}

.progress-description {
  color: var(--gray-600);
  font-size: 0.875rem;
  line-height: 1.5;
}
```

### Form Components

#### Input Field
```css
.form-field {
  margin-bottom: 1.5rem;
}

.form-label {
  display: block;
  font-weight: 600;
  color: var(--gray-700);
  margin-bottom: 0.5rem;
  font-size: 0.875rem;
}

.form-input {
  width: 100%;
  padding: 0.75rem 1rem;
  border: 2px solid var(--gray-200);
  border-radius: var(--radius-md);
  font-size: 1rem;
  line-height: 1.5;
  background: white;
  transition: all 0.2s ease;
}

.form-input:focus {
  outline: none;
  border-color: var(--primary-blue);
  box-shadow: 0 0 0 3px var(--primary-blue-light);
}

.form-input:invalid {
  border-color: var(--error-red);
}

.form-help {
  font-size: 0.75rem;
  color: var(--gray-500);
  margin-top: 0.25rem;
}

.form-error {
  font-size: 0.75rem;
  color: var(--error-red);
  margin-top: 0.25rem;
  display: flex;
  align-items: center;
  gap: 0.25rem;
}
```

#### Select Field
```css
.form-select {
  width: 100%;
  padding: 0.75rem 1rem;
  border: 2px solid var(--gray-200);
  border-radius: var(--radius-md);
  font-size: 1rem;
  background: white;
  cursor: pointer;
  transition: all 0.2s ease;
  appearance: none;
  background-image: url("data:image/svg+xml;charset=UTF-8,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='none' stroke='%236B7280' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3E%3Cpolyline points='6,9 12,15 18,9'/%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: right 1rem center;
  background-size: 1rem;
  padding-right: 3rem;
}

.form-select:focus {
  outline: none;
  border-color: var(--primary-blue);
  box-shadow: 0 0 0 3px var(--primary-blue-light);
}
```

### Navigation Components

#### Stepper Component
```css
.stepper {
  display: flex;
  align-items: center;
  margin-bottom: 2rem;
  overflow-x: auto;
  padding: 1rem 0;
}

.stepper-step {
  display: flex;
  align-items: center;
  flex-shrink: 0;
  min-width: 0;
}

.stepper-step:not(:last-child)::after {
  content: '';
  width: 2rem;
  height: 2px;
  background: var(--gray-200);
  margin: 0 1rem;
  border-radius: var(--radius-full);
}

.stepper-step.completed::after {
  background: var(--success-green);
}

.stepper-step.active::after {
  background: linear-gradient(90deg, var(--primary-blue) 50%, var(--gray-200) 50%);
}

.stepper-circle {
  width: 2.5rem;
  height: 2.5rem;
  border-radius: var(--radius-full);
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  font-size: 0.875rem;
  border: 2px solid var(--gray-200);
  background: white;
  color: var(--gray-500);
  transition: all 0.3s ease;
}

.stepper-step.completed .stepper-circle {
  background: var(--success-green);
  border-color: var(--success-green);
  color: white;
}

.stepper-step.active .stepper-circle {
  background: var(--primary-blue);
  border-color: var(--primary-blue);
  color: white;
  transform: scale(1.1);
}

.stepper-label {
  margin-left: 0.75rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: var(--gray-600);
  white-space: nowrap;
}

.stepper-step.active .stepper-label {
  color: var(--primary-blue);
  font-weight: 600;
}

.stepper-step.completed .stepper-label {
  color: var(--success-green);
}
```

### Status Components

#### Trust Badges
```css
.trust-badges {
  display: flex;
  gap: 1.5rem;
  align-items: center;
  flex-wrap: wrap;
  margin: 2rem 0;
}

.trust-badge {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1rem;
  background: var(--gray-50);
  border: 1px solid var(--gray-200);
  border-radius: var(--radius-md);
  font-size: 0.875rem;
  color: var(--gray-700);
}

.trust-badge-icon {
  color: var(--success-green);
  font-size: 1.25rem;
}

.security-badge {
  background: var(--success-green-light);
  border-color: var(--success-green);
  color: var(--success-green);
}

.legal-badge {
  background: var(--primary-blue-light);
  border-color: var(--primary-blue);
  color: var(--primary-blue);
}
```

---

## Page Specifications

### 1. Landing/Dashboard Page

#### Layout Structure
```
Header (Navigation)
├── Hero Section
├── Value Proposition Cards
├── Document Types Overview
├── Trust Indicators
├── Testimonials
└── Footer
```

#### Hero Section
- **Background**: Subtle gradient from white to light blue
- **Content**: Large headline, subheadline, primary CTA
- **Visual**: Family illustration or photo (warm, diverse)
- **CTA**: "Start Your Estate Plan" (prominent blue button)

#### Value Proposition Cards
```css
.value-prop-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 2rem;
  margin: 4rem 0;
}

.value-prop-card {
  text-align: center;
  padding: 2rem;
  background: white;
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
  transition: transform 0.3s ease;
}

.value-prop-card:hover {
  transform: translateY(-4px);
  box-shadow: var(--shadow-lg);
}

.value-prop-icon {
  width: 4rem;
  height: 4rem;
  background: var(--primary-blue-light);
  border-radius: var(--radius-full);
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 1.5rem;
  color: var(--primary-blue);
  font-size: 1.5rem;
}
```

### 2. Document Creation Wizard

#### Wizard Layout
- **Sidebar**: Progress stepper (desktop) / Top progress bar (mobile)
- **Main Content**: Form content with clear section headers
- **Bottom Bar**: Navigation buttons (Back/Next/Save)

#### Multi-Step Flow Structure
1. **Document Type Selection** (Cards with pricing)
2. **Personal Information** (Basic details form)
3. **Family & Beneficiaries** (Dynamic form with add/remove)
4. **Assets & Property** (Categorized input sections)
5. **Distribution Preferences** (Interactive allocation)
6. **Legal Preferences** (Checkboxes and options)
7. **Review & Confirm** (Summary with edit links)
8. **Payment & Completion** (Secure payment form)

#### Wizard Step Template
```css
.wizard-container {
  max-width: 1024px;
  margin: 0 auto;
  padding: 2rem 1rem;
  min-height: calc(100vh - 4rem);
  display: flex;
  flex-direction: column;
}

.wizard-header {
  margin-bottom: 2rem;
}

.wizard-title {
  font-size: 1.875rem;
  font-weight: 600;
  color: var(--gray-800);
  margin-bottom: 0.5rem;
}

.wizard-subtitle {
  color: var(--gray-600);
  font-size: 1.125rem;
  line-height: 1.6;
}

.wizard-content {
  flex: 1;
  margin-bottom: 2rem;
}

.wizard-navigation {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem 0;
  border-top: 1px solid var(--gray-200);
}

.wizard-nav-left {
  display: flex;
  gap: 1rem;
}

.wizard-nav-right {
  display: flex;
  gap: 1rem;
}
```

### 3. Inheritance Calculator

#### Calculator Layout
- **Input Panel**: Left side with form inputs
- **Visualization Panel**: Right side with charts/graphics
- **Results Summary**: Bottom section with breakdown

#### Interactive Elements
```css
.calculator-container {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
  margin: 2rem 0;
}

.calculator-inputs {
  background: white;
  padding: 2rem;
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-md);
}

.calculator-visual {
  background: white;
  padding: 2rem;
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-md);
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

.asset-input-group {
  margin-bottom: 2rem;
  padding: 1.5rem;
  background: var(--gray-50);
  border-radius: var(--radius-md);
  border-left: 4px solid var(--primary-blue);
}

.beneficiary-row {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1rem;
  padding: 1rem;
  background: white;
  border-radius: var(--radius-md);
  border: 1px solid var(--gray-200);
}

.percentage-slider {
  flex: 1;
  margin: 0 1rem;
}
```

### 4. Document Management Dashboard

#### Dashboard Grid Layout
```css
.document-dashboard {
  display: grid;
  grid-template-columns: 1fr 300px;
  gap: 2rem;
  margin: 2rem 0;
}

.documents-main {
  min-height: 600px;
}

.documents-sidebar {
  background: var(--gray-50);
  padding: 1.5rem;
  border-radius: var(--radius-lg);
  height: fit-content;
}

.document-list {
  display: grid;
  gap: 1rem;
}

.document-item {
  background: white;
  padding: 1.5rem;
  border-radius: var(--radius-md);
  border: 1px solid var(--gray-200);
  display: flex;
  justify-content: space-between;
  align-items: center;
  transition: all 0.2s ease;
}

.document-item:hover {
  box-shadow: var(--shadow-md);
  border-color: var(--primary-blue);
}

.document-info {
  flex: 1;
}

.document-title {
  font-weight: 600;
  color: var(--gray-800);
  margin-bottom: 0.25rem;
}

.document-meta {
  font-size: 0.875rem;
  color: var(--gray-500);
  display: flex;
  gap: 1rem;
}

.document-status {
  padding: 0.25rem 0.75rem;
  border-radius: var(--radius-full);
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.status-draft {
  background: var(--warning-amber-light);
  color: var(--warning-amber);
}

.status-complete {
  background: var(--success-green-light);
  color: var(--success-green);
}

.status-review {
  background: var(--primary-blue-light);
  color: var(--primary-blue);
}
```

### 5. Pricing Plans Page

#### Pricing Grid Layout
```css
.pricing-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 4rem 1rem;
}

.pricing-header {
  text-align: center;
  margin-bottom: 4rem;
}

.pricing-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(320px, 1fr));
  gap: 2rem;
  margin-bottom: 4rem;
}

.pricing-card {
  background: white;
  border: 2px solid var(--gray-200);
  border-radius: var(--radius-xl);
  padding: 2.5rem;
  text-align: center;
  position: relative;
  transition: all 0.3s ease;
}

.pricing-card.featured {
  border-color: var(--primary-blue);
  transform: scale(1.05);
  box-shadow: var(--shadow-xl);
}

.pricing-badge {
  position: absolute;
  top: -0.75rem;
  left: 50%;
  transform: translateX(-50%);
  background: var(--primary-blue);
  color: white;
  padding: 0.5rem 1.5rem;
  border-radius: var(--radius-full);
  font-size: 0.875rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.pricing-title {
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--gray-800);
  margin-bottom: 1rem;
}

.pricing-amount {
  font-size: 3rem;
  font-weight: 800;
  color: var(--primary-blue);
  margin-bottom: 0.5rem;
  line-height: 1;
}

.pricing-period {
  color: var(--gray-500);
  font-size: 1rem;
  margin-bottom: 2rem;
}

.pricing-features {
  list-style: none;
  padding: 0;
  margin: 0 0 2.5rem 0;
  text-align: left;
}

.pricing-feature {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 1rem;
  color: var(--gray-600);
}

.pricing-feature-icon {
  color: var(--success-green);
  font-size: 1.125rem;
  flex-shrink: 0;
}
```

---

## User Flows

### Primary User Flow: Creating a Will

1. **Landing** → Click "Create Will" CTA
2. **Document Selection** → Choose Will package
3. **Account Setup** → Quick registration/login
4. **Personal Info** → Basic details form
5. **Family Details** → Add spouse, children, beneficiaries
6. **Assets** → Categorize and value assets
7. **Distribution** → Allocate assets to beneficiaries
8. **Guardianship** → Select guardians for minors (if applicable)
9. **Executor** → Choose executor and backup
10. **Legal Options** → Additional legal preferences
11. **Review** → Summary with edit options
12. **Payment** → Secure checkout
13. **Completion** → Download/email documents

### Secondary Flow: Using Inheritance Calculator

1. **Landing** → Click "Free Calculator" CTA
2. **Calculator Page** → Input assets and beneficiaries
3. **Results** → View distribution visualization
4. **Lead Capture** → Email for detailed report
5. **Upsell** → Promote document creation services

### User Flow: Document Management

1. **Dashboard** → View all documents
2. **Document Detail** → Click on specific document
3. **Edit Mode** → Make changes to document
4. **Version Control** → Save as new version
5. **Download/Share** → Access final documents

---

## Implementation Guidelines

### MudBlazor Component Mapping

#### Recommended MudBlazor Components
```csharp
// Buttons
MudButton Color="Color.Primary" Variant="Variant.Filled"
MudButton Color="Color.Primary" Variant="Variant.Outlined" // Secondary
MudButton Variant="Variant.Text" // Ghost

// Forms
MudTextField
MudSelect<T>
MudCheckBox
MudRadioGroup<T>
MudDatePicker
MudNumericField<T>

// Layout
MudGrid
MudItem
MudPaper // For cards
MudContainer
MudStack

// Navigation
MudStepper
MudTabs
MudBreadcrumbs

// Data Display
MudTable<T>
MudChip
MudProgressLinear
MudProgressCircular
MudAlert

// Feedback
MudDialog
MudSnackbar
MudTooltip
```

### Responsive Breakpoints
```css
/* Mobile First Approach */
@media (min-width: 640px) { /* sm */ }
@media (min-width: 768px) { /* md */ }
@media (min-width: 1024px) { /* lg */ }
@media (min-width: 1280px) { /* xl */ }
```

### Performance Considerations
- Lazy load non-critical images
- Use MudBlazor's built-in virtualization for large lists
- Implement proper loading states
- Optimize form validation for real-time feedback
- Use progressive enhancement for complex interactions

### Accessibility Requirements
- WCAG 2.1 AA compliance
- Proper heading hierarchy
- Alt text for all images
- Keyboard navigation support
- Screen reader compatibility
- Color contrast ratios 4.5:1 minimum
- Focus indicators on all interactive elements

### Mobile-First Considerations
- Touch targets minimum 44px
- Thumb-friendly navigation
- Simplified forms on mobile
- Progressive disclosure of complex information
- Swipe gestures for stepper navigation
- Bottom sheet modals for mobile actions

---

## Trust and Security Visual Elements

### Security Indicators
```css
.security-banner {
  background: linear-gradient(135deg, var(--success-green-light), var(--primary-blue-light));
  padding: 1rem 2rem;
  border-radius: var(--radius-md);
  display: flex;
  align-items: center;
  gap: 1rem;
  margin: 2rem 0;
}

.security-icon {
  font-size: 1.5rem;
  color: var(--success-green);
}

.security-text {
  font-weight: 600;
  color: var(--gray-800);
}
```

### Legal Compliance Badges
- Attorney-reviewed documents
- State-specific legal compliance
- Bank-level encryption
- Secure document storage
- HIPAA compliance (if applicable)

### Trust Building Elements
- Professional headshots of legal team
- Bar association memberships
- Client testimonials with photos
- Security certifications
- Transparent pricing
- Money-back guarantee

---

This comprehensive design system provides the foundation for building a trustworthy, professional estate planning application that guides users through complex legal processes with confidence and clarity.