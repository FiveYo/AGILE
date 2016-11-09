using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FastDelivery_Library.Modele;

namespace FastDelivery_Library.PatternUndoRedo
{
    public class UndoRedo
    {
        private Stack<ModifyTournee> _UndoActionsContainer = new Stack<ModifyTournee>();
        private Stack<ModifyTournee> _RedoActionsContainer = new Stack<ModifyTournee>();

        private Tournee tournee;
        private Modele.Carte carte;
        public void Undo(int level)
        {
            for (int i = 1; i < level; i++)
            {
                if (_UndoActionsContainer.Count == 0)  return;

                ModifyTournee UndoStruct = _UndoActionsContainer.Pop();

                if (UndoStruct.action==ActionType.supprimer)
                {
                    tournee.AddLivraison(UndoStruct.carte,UndoStruct.livraison,UndoStruct.index);
                    RedoPushInUnDoForDelete(UndoStruct.carte, UndoStruct.livraison, UndoStruct.index);
                }
                else if (UndoStruct.action==ActionType.ajouter)
                {
                    tournee.DelLivraison(UndoStruct.carte, UndoStruct.livraison);
                    RedoPushInUnDoForAdd(UndoStruct.carte, UndoStruct.livraison, UndoStruct.index);
                }
                else if (UndoStruct.action==ActionType.modifier)
                {
                    //TODO
                    // add RedoPushUnDoModify
                }

            }
        }
        public void Redo(int level)
        {
            for (int i = 1; i < level; i++)
            {
                if (_RedoActionsContainer.Count == 0) return;

                ModifyTournee RedoStruct = _RedoActionsContainer.Pop();

                if (RedoStruct.action == ActionType.supprimer)
                {
                    
                    ModifyTournee ChangeforDelete = this.MakeChangeObjectForDelete(RedoStruct.carte, RedoStruct.livraison, RedoStruct.index);
                    _UndoActionsContainer.Push(ChangeforDelete);
                    tournee.DelLivraison(RedoStruct.carte, RedoStruct.livraison);

                }
                else if (RedoStruct.action == ActionType.ajouter)
                {
                    ModifyTournee ChangeforAdd = this.MakeChangeObjectForAdd(RedoStruct.carte, RedoStruct.livraison, RedoStruct.index);
                    _UndoActionsContainer.Push(ChangeforAdd);
                    tournee.AddLivraison(RedoStruct.carte, RedoStruct.livraison, RedoStruct.index);
                }
                else if (RedoStruct.action == ActionType.modifier)
                {
                    //TODO
                    // add RedoPushUnDoModify
                }
            }
        }

        public ModifyTournee MakeChangeObjectForAdd(Modele.Carte carte, Livraison livraison, int index)
        {
            ModifyTournee UndoStruct = new ModifyTournee();
            UndoStruct.index = index;
            UndoStruct.carte = carte;
            UndoStruct.livraison = livraison;
            return UndoStruct;
        }
        public ModifyTournee MakeChangeObjectForDelete(Modele.Carte carte, Livraison livraison, int index)
        {
            ModifyTournee UndoStruct = new ModifyTournee();
            UndoStruct.index = index;
            UndoStruct.carte = carte;
            UndoStruct.livraison = livraison;
            return UndoStruct;
        }
        public ModifyTournee MakeChangeObjectForModify(Modele.Carte carte, Livraison livraison, int index)
        {
            ModifyTournee UndoStruct = new ModifyTournee();
            //TODO
            return UndoStruct;
        }
        public void RedoPushInUnDoForAdd(Modele.Carte carte, Livraison livraison, int index)
        {
            ModifyTournee RedoStruct = new ModifyTournee();
            RedoStruct.action = ActionType.ajouter;
            RedoStruct.carte = carte;
            RedoStruct.livraison = livraison;
            RedoStruct.index = index;
            _RedoActionsContainer.Push(RedoStruct);
        }
        public void RedoPushInUnDoForDelete(Modele.Carte carte, Livraison livraison, int index)
        {
            ModifyTournee RedoStruct = new ModifyTournee();
            RedoStruct.action = ActionType.supprimer;
            RedoStruct.carte = carte;
            RedoStruct.livraison = livraison;
            RedoStruct.index = index;
            _RedoActionsContainer.Push(RedoStruct);

        }
        public void RedoPushInUnDoForModify(Modele.Carte carte, Livraison livraison, int index)
        {
            //TODO

        }

        public void clearUnRedo()
        {
            _UndoActionsContainer.Clear();
        }
        public bool IsUndoPossible()
        {
            if (_UndoActionsContainer.Count != 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool IsRedoPossible()
        {
            if (_RedoActionsContainer.Count != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
