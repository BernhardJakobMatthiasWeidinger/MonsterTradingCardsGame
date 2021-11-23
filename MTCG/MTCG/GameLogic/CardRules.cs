﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MTCG.GameLogic {
    public class CardRule {
        public static Tuple<double, double> compareUniqueRule(Card card1, Card card2, double calcDamage1, double calcDamage2) {
            Tuple<double, double> res = new Tuple<double, double>(card1.damage, card2.damage);

            if (card1.name.ToLower().Contains("goblin") && card2.name.ToLower().Contains("dragon") ||
                card1.name.ToLower().Contains("ork") && card2.name.ToLower().Contains("wizard") ||
                card1.name.ToLower().Contains("spell") && card2.name.ToLower().Contains("kraken") ||
                card1.name.ToLower().Contains("dragon") && card2.name.ToLower().Contains("fireelf")) {
                res = new Tuple<double, double>(0, calcDamage2);
            } else if (card1.name.ToLower().Contains("wizard") && card2.name.ToLower().Contains("ork") ||
                card1.name.ToLower().Contains("kraken") && card2.name.ToLower().Contains("spell") ||
                card1.name.ToLower().Contains("fireelf") && card2.name.ToLower().Contains("dragon") ||
                card1.name.ToLower().Contains("dragon") && card2.name.ToLower().Contains("goblin")) {
                res = new Tuple<double, double>(calcDamage1, 0);
            } else if (card1.name.ToLower().Contains("knight") && card2.name.ToLower().Contains("waterspell")) {
                res = new Tuple<double, double>(calcDamage1, 9999);
            } else if (card1.name.ToLower().Contains("waterspell") && card2.name.ToLower().Contains("knight")) {
                res = new Tuple<double, double>(9999, calcDamage2);
            } 

            return res;
        }

        public static Tuple<double, double> compareElementType(Card card1, Card card2) {
            Tuple<double, double> res = new Tuple<double, double>(card1.damage, card2.damage);

            if (card1.GetType().Name == "SpellCard" || card2.GetType().Name == "SpellCard") {
                //if card1 has disadvantage type
                if (card1.elementType == ElementType.fire && card2.elementType == ElementType.water ||
                    card1.elementType == ElementType.water && card2.elementType == ElementType.normal ||
                    card1.elementType == ElementType.normal && card2.elementType == ElementType.fire) {
                    res = new Tuple<double, double>(card1.damage / 2, card2.damage * 2);
                    //if card2 has disadvantage type
                } else if (card1.elementType == ElementType.fire && card2.elementType == ElementType.normal ||
                    card1.elementType == ElementType.water && card2.elementType == ElementType.fire ||
                    card1.elementType == ElementType.normal && card2.elementType == ElementType.water) {
                    res = new Tuple<double, double>(card1.damage * 2, card2.damage / 2);
                } 
            }

            return res;
        }
    }
}