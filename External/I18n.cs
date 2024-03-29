﻿namespace Shockah.Kokoro;

public static class I18n
{
	public static string ScorchingGlossaryName => "Scorching";
	public static string ScorchingGlossaryDescription => "The object takes damage each turn. If this object is destroyed by a ship's <c=action>ATTACK</c> or <c=action>LAUNCH</c>, the ship gains {0} <c=status>HEAT</c>.";
	public static string ScorchingGlossaryAltDescription => "The object takes damage each turn. If this object is destroyed by a ship's <c=action>ATTACK</c> or <c=action>LAUNCH</c>, the ship gains <c=status>HEAT</c>.";

	public static string WormStatusName => "WORM";
	public static string WormStatusDescription => "Cancels {0} intents at the start of the player's turn. <c=downside>Decreases by 1 at end of turn.</c>";
	public static string WormStatusAltGlossaryDescription => "Cancels intents at the start of the player's turn. <c=downside>Decreases by 1 at end of turn.</c>";

	public static string OxidationStatusName => "OXIDATION";
	public static string OxidationStatusDescription => "If oxidation is {0} or more at end of turn, gain 1 <c=status>CORRODE</c> and set oxidation to 0.";

	public static string ConditionalActionName => "CONDITIONAL";
	public static string ConditionalActionDescription => "This action will only trigger if {0}.";
	public static string ConditionalHandDescription => "<c=boldPink>{0}</c> cards in your hand";
	public static string ConditionalXDescription => "<c=action>X</c>";
	public static string ConditionalHasStatusDescription => "you have {0}";
	public static string ConditionalEquationFormalEqualDescription => "{0} is {1}";
	public static string ConditionalEquationFormalNotEqualDescription => "{0} is not {1}";
	public static string ConditionalEquationFormalGreaterThanDescription => "{0} is greater than {1}";
	public static string ConditionalEquationFormalLessThanDescription => "{0} is less than {1}";
	public static string ConditionalEquationFormalGreaterThanOrEqualDescription => "{0} is greater than or equal to {1}";
	public static string ConditionalEquationFormalLessThanOrEqualDescription => "{0} is less than or equal to {1}";
	public static string ConditionalEquationStateEqualDescription => "you are at {1} {0}";
	public static string ConditionalEquationStateNotEqualDescription => "you are not at {1} {0}";
	public static string ConditionalEquationStateGreaterThanDescription => "you are higher than {1} {0}";
	public static string ConditionalEquationStateLessThanDescription => "you are lower than {1} {0}";
	public static string ConditionalEquationStateGreaterThanOrEqualDescription => "you are at least at {1} {0}";
	public static string ConditionalEquationStateLessThanOrEqualDescription => "you are at most at {1} {0}";
	public static string ConditionalEquationPossessionEqualDescription => "you have exactly {1} {0}";
	public static string ConditionalEquationPossessionNotEqualDescription => "you do not have exactly {1} {0}";
	public static string ConditionalEquationPossessionGreaterThanDescription => "you have more than {1} {0}";
	public static string ConditionalEquationPossessionLessThanDescription => "you have less than {1} {0}";
	public static string ConditionalEquationPossessionGreaterThanOrEqualDescription => "you have at least {1} {0}";
	public static string ConditionalEquationPossessionLessThanOrEqualDescription => "you have at most {1} {0}";
	public static string ConditionalEquationPossessionComparisonEqualDescription => "you have the same amount of {0} and {1}";
	public static string ConditionalEquationPossessionComparisonNotEqualDescription => "you have a different amount of {0} and {1}";
	public static string ConditionalEquationPossessionComparisonGreaterThanDescription => "you have more {0} than {1}";
	public static string ConditionalEquationPossessionComparisonLessThanDescription => "you have less {0} than {1}";
	public static string ConditionalEquationPossessionComparisonGreaterThanOrEqualDescription => "you have at least as much {0} as {1}";
	public static string ConditionalEquationPossessionComparisonLessThanOrEqualDescription => "you have at most as much {0} as {1}";

	public static string StatusPlayerCostActionName => "{0} COST";
	public static string StatusPlayerCostActionDescription => "Lose <c=keyword>{0}</c> <c=status>{1}</c>. If you don't have enough, this action does not happen.";
	public static string StatusEnemyCostActionName => "EXPLOIT {0}";
	public static string StatusEnemyCostActionDescription => "Remove <c=keyword>{0}</c> <c=status>{1}</c> from the enemy. If they don't have enough, this action does not happen.";
	public static string EnergyCostActionName => "ENERGY COST";
	public static string EnergyCostActionDescription => "Lose <c=keyword>{0}</c> extra <c=status>ENERGY</c>. If you don't have enough, this action does not happen.";

	public static string EnemyVariableHint => "<c=action>X</c> = The enemy's {0}{1}{2}{3}.";
	public static string EnergyVariableHint => "<c=action>X</c> = Your <c=status>ENERGY</c> after paying for this card{0}.";
	public static string EnergyGlossaryName => "ENERGY";
	public static string EnergyGlossaryDescription => "How much <c=energy>ENERGY</c> you have remaining this turn.";

	public static string ContinueActionName => "CONTINUE";
	public static string ContinueActionDescription => "Trigger the next actions. If this is not triggered, the next actions will not be either.";
	public static string StopActionName => "STOP";
	public static string StopActionDescription => "Stop triggering the next actions. If this is not triggered, the next actions will trigger as usual.";
}