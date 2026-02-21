namespace RpgEngine.Core.Tests;

using FluentAssertions;
using RpgEngine.Core.Graph;
using RpgEngine.Core.Models;

public class ConditionEvaluatorTests
{
    private GameState CreateState()
    {
        return new GameState
        {
            Player = new PlayerState
            {
                Level = 3,
                Strength = 14,
                Defense = 10,
                Hp = 20,
                Mana = 8,
                Inventory = new List<string> { "espada", "poção" }
            },
            Flags = new Dictionary<string, string>
            {
                { "ponte_consertada", "true" },
            }
        };
    }

    [Fact]
    public void Check_CondicaoNula_DeveRetornarTrue()
    {
        ConditionEvaluator.Check(null, new GameState()).Should().BeTrue();
    }

    [Fact]
    public void Check_CondicaoVazia_DeveRetornarTrue()
    {
        ConditionEvaluator.Check("", new GameState()).Should().BeTrue();
        ConditionEvaluator.Check(" ", new GameState()).Should().BeTrue();
    }

    [Fact]
    public void Check_HasItem_Presente_DeveRetornarTrue()
    {
        var state = CreateState();

        ConditionEvaluator.Check("has_item:espada", state).Should().BeTrue();
    }

    [Fact]
    public void Check_HasItem_Ausente_DeveRetornarFalse()
    {
        var state = CreateState();

        ConditionEvaluator.Check("has_item:arco", state).Should().BeFalse();
    }

    [Fact]
    public void Check_Flag_Ativa_DeveRetornarTrue()
    {
        var state = CreateState();

        ConditionEvaluator.Check("flag:ponte_consertada", state).Should().BeTrue();
    }

    [Fact]
    public void Check_Flag_Inativa_DeveRetornarFalse()
    {
        var state = CreateState();

        ConditionEvaluator.Check("flag:porta_aberta", state).Should().BeFalse();
    }

    [Fact]
    public void Check_Level_Suficiente_DeveRetornarTrue()
    {
        var state = CreateState();

        ConditionEvaluator.Check("level:3", state).Should().BeTrue();
        ConditionEvaluator.Check("level:1", state).Should().BeTrue();
    }

    [Fact]
    public void Check_Level_Insuficiente_DeveRetornarFalse()
    {
        var state = CreateState();

        ConditionEvaluator.Check("level:5", state).Should().BeFalse();
    }

    [Fact]
    public void Check_Stat_Suficiente_DeveRetornarTrue()
    {
        var state = CreateState();

        ConditionEvaluator.Check("stat:strength:14", state).Should().BeTrue();
        ConditionEvaluator.Check("stat:defense:10", state).Should().BeTrue();
    }

    [Fact]
    public void Check_Stat_Insuficiente_DeveRetornarFalse()
    {
        var state = CreateState();

        ConditionEvaluator.Check("stat:strength:18", state).Should().BeFalse();
    }

    [Fact]
    public void Check_Negacao_FlagAtiva_DeveRetornarFalse()
    {
        var state = CreateState();

        ConditionEvaluator.Check("!flag:ponte_consertada", state).Should().BeFalse();
    }

    [Fact]
    public void Check_Negacao_FlagInativa_DeveRetornarTrue()
    {
        var state = CreateState();

        ConditionEvaluator.Check("!flag:porta_destruida", state).Should().BeTrue();
    }

    [Fact]
    public void Check_CondicaoDesconhecida_DeveRetornarFalse()
    {
        var state = CreateState();

        ConditionEvaluator.Check("tipo_invalido:valor", state).Should().BeFalse();
    }
}