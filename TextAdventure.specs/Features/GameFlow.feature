Feature: Text Adventure game flow

  Scenario: Speler start in de startkamer
    Given een nieuw spel is gestart
    Then staat de speler in de kamer "Start"
    And is het spel niet afgelopen
    And heeft de speler nog niet gewonnen

  Scenario: Speler beweegt naar een bestaande kamer
    Given een nieuw spel is gestart
    When de speler naar het oosten gaat
    Then staat de speler in de kamer "Schatkamer"

  Scenario: Speler kan niet bewegen naar een richting zonder uitgang
    Given een nieuw spel is gestart
    When de speler naar het oosten gaat
    And de speler naar het oosten gaat
    Then staat de speler in de kamer "Schatkamer"
    And is het spel niet afgelopen

  Scenario: Speler kan de sleutel oppakken
    Given een nieuw spel is gestart
    When de speler naar het oosten gaat
    And de speler het item "Sleutel" oppakt
    Then heeft de speler het item "Sleutel"

  Scenario: Speler kan niet winnen zonder sleutel
    Given een nieuw spel is gestart
    When de speler naar het noorden gaat
    Then staat de speler in de kamer "Start"
    And heeft de speler nog niet gewonnen

  Scenario: Speler wint met de sleutel
    Given een nieuw spel is gestart
    When de speler naar het oosten gaat
    And de speler het item "Sleutel" oppakt
    And de speler naar het westen gaat
    And de speler naar het noorden gaat
    Then staat de speler in de kamer "De Uitgang"
    And heeft de speler gewonnen

  Scenario: Speler verliest in de dodelijke gang
    Given een nieuw spel is gestart
    When de speler naar het westen gaat
    Then is het spel afgelopen
    And heeft de speler niet gewonnen

  Scenario: Speler verliest tegen monster zonder zwaard
    Given een nieuw spel is gestart
    When de speler naar het zuiden gaat
    And de speler naar het zuiden gaat
    And de speler vecht
    Then is het spel afgelopen
    And heeft de speler niet gewonnen

  Scenario: Speler verslaat monster met zwaard
    Given een nieuw spel is gestart
    When de speler naar het zuiden gaat
    And de speler het item "Zwaard" oppakt
    And de speler naar het zuiden gaat
    And de speler vecht
    Then is het spel niet afgelopen
    And leeft het monster niet meer