# Release notes

This document contains the notes for individual releases of the _TeklaStructuralDesigner.RemotingAPI_ NuGet package.

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