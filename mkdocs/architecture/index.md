---
hide:
  - navigation
---

# Architecture & Design

Peek under the hood – how REslava.Result is built and how its source generators work.

<div class="grid cards" markdown>

-   :material-sitemap: __Complete Architecture__  
    Visual diagrams, component breakdown, and SOLID principles in action.
    [](complete-architecture)

-   :material-package: __Package Structure__  
    What each NuGet package contains and how they integrate.
    [](package-structure)

-   :material-cog: __How Generators Work__  
    The source generator pipeline – analysis, generation, build integration.
    [](complete-architecture#source-generators-reslavaresultsourcegenerators)

-   :simple-solid: __SOLID Architecture__  
    SOLID Principles Implementation.
    [](solid/solid-architecture)

-   :simple-uml: __REslava.Result Core Type Hierarchy__  
    UML class diagrams illustrating the core type hierarchy of REslava.Result – Reason & Error hierarchy, Result<T>, and advanced patterns like Maybe<T> and OneOf unions.
    [](solid/uml-v1.12.1-core)
    {: .is-featured }

-   :simple-uml: __REslava.Result.Sourcegenerators__  
    UML diagrams detailing the source generator architecture of REslava.Result – generator delegation, orchestrators, two‑phase attribute and code generation, and the extension methods produced for Result<T> and OneOf unions.
    [](solid/uml-v1.12.1-generators)

</div>