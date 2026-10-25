# Estate Planning Application - UI Design System

## Design Philosophy
The estate planning application should convey **trust, security, and professional competence** while remaining **approachable and user-friendly**. Users are dealing with important legal and financial decisions, so the interface must inspire confidence.

## Color Palette

### Primary Colors
- **Deep Blue (#1565C0)** - Trust, stability, professionalism
- **Forest Green (#2E7D32)** - Growth, security, money
- **Warm Gray (#5D4E75)** - Sophistication, balance

### Secondary Colors
- **Gold Accent (#F57C00)** - Premium, value, important actions
- **Success Green (#4CAF50)** - Completed states, positive outcomes
- **Warning Orange (#FF9800)** - Attention, review required
- **Error Red (#D32F2F)** - Issues, validation errors

### Neutral Palette
- **Charcoal (#333333)** - Primary text
- **Medium Gray (#666666)** - Secondary text
- **Light Gray (#F5F5F5)** - Backgrounds, dividers
- **Pure White (#FFFFFF)** - Content backgrounds

## Typography

### Headings
- **H1-H2**: Roboto Bold, 28-32px - Page titles
- **H3-H4**: Roboto Medium, 22-24px - Section headers
- **H5-H6**: Roboto Medium, 18-20px - Card titles

### Body Text
- **Body Large**: Roboto Regular, 16px - Primary content
- **Body Regular**: Roboto Regular, 14px - Secondary content
- **Caption**: Roboto Regular, 12px - Helper text, labels

## Key UI Components

### 1. Document Type Selection Cards
```css
.document-card {
  padding: 24px;
  border-radius: 12px;
  border: 2px solid transparent;
  transition: all 0.3s ease;
  cursor: pointer;
}

.document-card:hover {
  border-color: var(--primary-color);
  transform: translateY(-4px);
  box-shadow: 0 8px 25px rgba(21, 101, 192, 0.15);
}
```

**Layout Structure:**
- Icon (64px, centered)
- Document type title
- Price (prominent, colored)
- Brief description
- Time estimate
- "Popular" badge (if applicable)

### 2. Multi-Step Wizard
**Progress Indicator:**
- Horizontal stepper with 4-5 steps
- Completed steps in green
- Current step highlighted in primary color
- Future steps in gray

**Step Content:**
- Clear step title and description
- Form fields with proper validation
- Helper text and tooltips for complex concepts
- Progress summary sidebar

### 3. Inheritance Calculator
**Visual Elements:**
- Asset input cards with icons
- Beneficiary rows with percentage sliders
- Real-time calculation display
- Pie chart or bar chart for distribution visualization
- Warning indicators for percentage mismatches

### 4. Trust & Security Indicators
- SSL encryption badge
- Legal compliance icons
- Scholar certification mentions
- Lifetime updates guarantee
- Money-back guarantee

## Page Layouts

### Dashboard/Landing Page
```
┌─────────────────────────────────────────┐
│ Header: Logo, Nav, User Menu            │
├─────────────────────────────────────────┤
│ Hero Section: Main CTA & Value Prop     │
├─────────────────────────────────────────┤
│ Document Type Cards (Grid Layout)       │
├─────────────────────────────────────────┤
│ Inheritance Calculator Promo            │
├─────────────────────────────────────────┤
│ Recent Documents (Table)                │
├─────────────────────────────────────────┤
│ Trust Indicators Footer                 │
└─────────────────────────────────────────┘
```

### Document Creation Wizard
```
┌─────────────────────────────────────────┐
│ Header with Progress Stepper            │
├─────────────────┬───────────────────────┤
│ Main Content    │ Progress Summary      │
│ Area           │ Sidebar               │
│                │                       │
│ Form Fields    │ - Steps completed     │
│ & Inputs       │ - Estimated time      │
│                │ - Help resources      │
├─────────────────┴───────────────────────┤
│ Navigation: Previous | Next             │
└─────────────────────────────────────────┘
```

### Inheritance Calculator
```
┌─────────────────────────────────────────┐
│ Calculator Header & Reset Button        │
├─────────────────┬───────────────────────┤
│ Assets Section  │ Live Results Panel    │
├─────────────────┤                       │
│ Debts Section   │ - Total Assets        │
├─────────────────┤ - Net Estate          │
│ Beneficiaries   │ - Distribution Chart  │
│ Section         │                       │
├─────────────────┴───────────────────────┤
│ Call-to-Action: Create Estate Plan      │
└─────────────────────────────────────────┘
```

## Interactive Elements

### Form Controls
- **Input Fields**: Rounded corners (8px), subtle shadow on focus
- **Dropdowns**: Custom styled with estate-specific options
- **Buttons**: 
  - Primary: Solid background, white text
  - Secondary: Outline style with primary color
  - Tertiary: Text-only with hover effects

### Hover States
- Subtle color shifts (10% darker/lighter)
- Smooth transitions (300ms ease)
- Shadow elevation changes
- Micro-animations for feedback

### Loading States
- Skeleton screens for content areas
- Spinner overlays for actions
- Progress bars for multi-step processes

## Mobile Responsiveness

### Breakpoints
- **Mobile**: < 768px
- **Tablet**: 768px - 1024px  
- **Desktop**: > 1024px

### Mobile Adaptations
- Stack document cards vertically
- Collapse wizard sidebar into expandable sections
- Simplify calculator layout to single column
- Touch-optimized button sizes (44px minimum)
- Swipe gestures for wizard navigation

## Accessibility Features
- WCAG 2.1 AA compliance
- High contrast mode support
- Keyboard navigation
- Screen reader optimization
- Focus indicators
- Alt text for all images and icons

## Micro-Interactions
- **Form Validation**: Real-time feedback with gentle color changes
- **Step Completion**: Check marks with subtle animation
- **Calculation Updates**: Smooth number transitions
- **Card Selections**: Gentle bounce effect
- **Button Clicks**: Brief press animation

## Visual Hierarchy Rules
1. **Most Important**: Large headings, primary buttons, prices
2. **Important**: Section headings, form labels, key metrics
3. **Supporting**: Body text, descriptions, helper text
4. **Background**: Dividers, borders, subtle backgrounds

This design system creates a professional, trustworthy interface that guides users through complex estate planning decisions while maintaining clarity and ease of use.