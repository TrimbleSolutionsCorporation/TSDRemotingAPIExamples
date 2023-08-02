using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSD.API.Remoting;
using TSD.API.Remoting.Common;
using TSD.API.Remoting.Report;
using TSD.API.Remoting.Structure;
using TSD.API.Remoting.Units;

namespace ExtractingResultsTableItem
{
	public class ResultsValuesOutputter
	{
		#region Constructors

		public ResultsValuesOutputter( IMember member, string lineItemString, IApplication tsdInstance )
		{
			LineItemString = lineItemString;
			Member = member;
			UnitConverter = tsdInstance.UnitConverter;
		}

		#endregion

		#region Properties

		private string LineItemString { get; }

		private IMember Member { get; }

		private IUnitConverter UnitConverter { get; }

		#endregion

		#region Methods

		public async Task Output()
		{
			var results = await Member.GetCalculationsAsync( CheckResultType.Static );

			await ProcessCalculationsNodeList( results.Nodes, "" );
			await ProcessItems( results.Items, "" );
		}

		private async Task ProcessCalculationsNodeList( IReadOnlyList<ICalculationsNode> nodes, string hierarchyString )
		{
			if( !nodes.Any() )
			{
				return;
			}

			foreach( var node in nodes )
			{
				string hierarchyStringNode = hierarchyString + " > " + node.Name;

				await ProcessCalculationsNodeList( node.Nodes, hierarchyStringNode );
				await ProcessItems( node.Items, hierarchyStringNode );
			}
		}

		private async Task ProcessItems( IReadOnlyList<IReportItem> items, string hierarchyString )
		{
			if( !items.Any() )
				return;

			foreach( var item in items )
			{
				await ProcessItem( item, hierarchyString );
			}
		}

		private async Task ProcessItem( IReportItem item, string hierarchyString )
		{
			if( item is IReportTable table )
			{
				await ProcessTable( table, hierarchyString );
			}
		}

		private async Task ProcessTable( IReportTable table, string hierarchyString )
		{
			foreach( var line in table.Lines )
			{
				await ProcessLine( line, hierarchyString );
			}
		}

		private async Task ProcessLine( IReportTableLine line, string hierarchyString )
		{
			if( line.Table != null )
			{
				await ProcessTable( line.Table, hierarchyString );
			}

			var item = line.Items.FirstOrDefault( i => i is IReportTableLineItemText || i is IReportTableLineItemCombined );

			if( item != null )
			{
				string text = "";

				if( item is IReportTableLineItemText textItem )
					text = textItem.Text;
				else if( item is IReportTableLineItemCombined combinedItem )
					text = combinedItem.Text;

				if( !text.Contains( LineItemString ) )
				{
					return;
				}

				// Text matches that being searched for! Output the result value.
				await OutputResultValue( line, hierarchyString );
			}
		}

		private async Task OutputResultValue( IReportTableLine line, string hierarchyString )
		{
			// Assume we are looking for a line that contains just one value and one quantity
			double value = 0.0;
			Quantity quantity = Quantity.Unknown;
			string text = "";
			bool textFound, valueFound, quantityFound;
			textFound = valueFound = quantityFound = false;
			foreach( var item in line.Items )
			{
				if( !textFound )
				{
					if( item is IReportTableLineItemCombined combinedItem )
					{
						text = combinedItem.Text;
						textFound = true;
					}
					else if( item is IReportTableLineItemText textItem )
					{
						text = textItem.Text;
						textFound = true;
					}
				}

				if( !valueFound )
				{
					if( item is IReportTableLineItemCombined combinedItem )
					{
						value = combinedItem.Value;
						valueFound = true;
					}
					else if( item is IReportTableLineItemDoubleValue doubleItem )
					{
						value = doubleItem.Value;
						valueFound = true;
					}
					else if( item is IReportTableLineItemLongValue longItem )
					{
						value = longItem.Value;
						valueFound = true;
					}
				}

				if( !quantityFound )
				{
					if( item is IReportTableLineItemCombined combinedItem )
					{
						quantity = combinedItem.Quantity;
						quantityFound = true;
					}
					else if( item is IReportTableLineItemQuantity quantityItem )
					{
						quantity = quantityItem.Quantity;
						quantityFound = true;
					}
				}
			}

			if( !valueFound || !quantityFound )
			{
				// Not a valid value to output
				return;
			}

			var units = await UnitConverter.GetUnitsAsync( quantity, SystemType.Metric );
			var unit = units.First();

			if( unit != null )
			{
				Console.WriteLine( hierarchyString );

				Console.WriteLine( $"\t{text} {(await UnitConverter.FromBaseAsync( new[] { value }, unit )).First()} [{unit.Name}]" );
			}
		}

		#endregion
	}
}
