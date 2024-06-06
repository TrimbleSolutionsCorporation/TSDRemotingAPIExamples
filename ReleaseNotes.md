# Release notes

This document contains the notes for individual releases of the _TeklaStructuralDesigner.RemotingAPI_ NuGet package.

## Version 24.1.0

* A new overload for `IModel.CreateMemberAttributeSetAsync` method has been added that allows users to specify the type of created member.
  * This change also allows users to set the section of created members.
  * The original overload is therefore redundant, has been marked obsolete and will be removed in version 26.0.0.
* The property `IsWindward` has been added to `ISnowLoadDataDrift`.
* The property `DistanceToRidge` has been added to `ISnowLoadDataRainOnSnow`.

## Version 24.0.0

* A dedicated combination class for staged construction has been added.
* A dedicated set of staged construction analysis types has been added.
* The ability to access analysis types used in element design has been added (see `GetAnalysisTypesForElementDesignAsync` in `Structure.IModel`).
* A new masonry material type has been added (see `IMasonry`).
  * Unfortunately, this _breaks the backwards compatibility_ so in order to work with TSD of version 24.0 and later, the API of at least version 24.0 needs to be used.

## Version 23.4.0

* A bug preventing connection to the API when using trial version of TSD has been fixed.

## Version 23.3.0

* The ability to access connections via `Structure.IModel` interface has been added.
* The ability to access floor analysis models via `Structure.IModel` interface has been added.
* The floor analysis results (vibration modes and mode shapes by node) can be now accessed via solver model of floor analysis model (`GetVibrationModesAsync` and `GetModeShapesByNodeAsync` respectively).
* The ability to access base plate data via `IBasePlate` interface has been added.
* The property `GlobalRotationAngle` has been added to `IMemberSpan`.
* The property `CombinationSpeciality` has been added to `ICombination`.
* There is now an option to close an instance of Tekla Structural Designer application by calling `IApplication.StopAsync()`. This is only allowed for instances that were started via the API and the stop action can only be done from the client that started the instance.
* A bug preventing access to concrete sections with circular holes has been fixed.
* A bug preventing access to user-defined attributes has been fixed.

## Version 23.2.0

* There is now a new way to peek at running instances of Tekla Structural Designer at the local machine instead of having to connect to them right away (`ApplicationFactory.GetRunningApplicationInfosAsync`). The provided `IApplicationInfo` then can be used to connect to the associated instance of Tekla Structural Designer using a new overload of method (`ApplicationFactory.ConnectToRunningApplicationAsync`).
* The ability to access calculations of an object has been added (see `IHaveCalculations`).
* A bug causing crash when accessing `DepthToThicknessRatio`, `BucklingParameter` and/or `TorsionalIndex` properties of `IPlatedISection` has been fixed. The properties now return `double.NaN` instead and a new interface `IPlatedISectionV2` has been added that has these properties as nullable (with `null` value instead of `double.NaN`). The old interface (`IPlatedISection`) has been made obsolete.
* The ability to query units and convert values has been added (`IUnitConverter` in `IApplication`).
* The ability to access current selection has been added (see `ISelectionItem`).

## Version 23.1.0

* The ability to run the _"Analyze All"_ and _"Design All"_ commands has been added.
* Westok web openings have been exposed via API.
  * The following properties of `IWebOpening` interface can now be accessed via the `Stiffener` property and were therefore made obsolete:
    * `Stiffening`
    * `StiffenerDepth`
    * `StiffenerThickness`
    * `StiffenerLength`
    * `EdgeToStiffenerCenter`
  * All these obsolete properties will be removed in version 25.0.0.
* The stiffeners of web openings can be now added (resp. removed) via dedicated `Add` and `Remove` methods.
  * Property `IsStiffened` can therefore no longer have its value set and will be changed to `IReadOnlyProperty` in version 25.0.0.
* The property `FilletWeldLegLength` of simple web opening stiffener has been exposed.
* The following properties related to splices have been added to `ISteelColumnStackData`
  * `HasSplice`
  * `SpliceOffset`
* The settings related to export of continuous objects have been exposed via API (under `IModelSettings.BimSettings`).

## Version 23.0.0

This is the first public version of the NuGet package.

The aim of the API is to allow its users to interact with the Tekla Structural Designer from within code. The following functionality is provided by the API:
* Control of Tekla Structural Designer instances
  * Start a new instance of Tekla Structural Designer
  * Get currently running instances of Tekla Structural Designer
  * Connect to/disconnect from an instance of Tekla Structural Designer
* Control of document within Tekla Structural Designer
  * Create a new document
  * Open existing document (from file or stream)
  * Save current document
  * Close current document
* Access to information about sessions
* Access to project details
* Access to the structure model
  * Access to model entities and the ability to create/modify them
    * Architectural grid
    * Architectural grid lines
    * Construction points
    * Construction point groups
    * Construction planes (horizontal, vertical, sloped)
    * Foundations
      * Pad bases
      * Pile caps
      * Piles
    * Members
      * Beams
      * Braces
      * Columns
    * Portal frames
      * Members
      * Member spans
      * Rafters
      * Supports
      * Valleys
    * Roofs
    * Slabs
    * Slab items
    * Slab openings
    * Slab patches
    * Strips
    * Structural walls
    * Structural wall openings
    * Supports
    * Web openings
    * Wind walls
  * Access to loading and the ability to create/modify it
    * Loads
    * Load cases
    * Combinations
    * Envelopes
  * Access to material properties of model entities
    * Sections
    * Materials
    * Reinforcement
    * Profiles
    * Gauges
  * Access to solver model
    * Analysis results
    * Displacements
    * Forces
      * Nodal forces
      * Nodal mesh forces
      * Element end forces
      * Mesh forces
      * Result line forces
      * Wall line forces
      * Wall line station forces
    * Objects
      * Nodes
      * Supports
      * 1D elements
      * 2D elements
      * Panels
      * Diaphragms
      * Result lines
      * Stations
      * Wall lines
  * Access to user-defined attributes
    * Simple attributes
    * Embedded files
  * Access to information about embodied carbon in model entities
  * Control of validation
    * Access to the latest validation status
    * Means to run validation
  * Control of design
    * Access to the latest design status
    * Means to run design
