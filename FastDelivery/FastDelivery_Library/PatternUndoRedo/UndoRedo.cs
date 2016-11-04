using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    // add RedoPushUndoDelete
                }
                else if (UndoStruct.action==ActionType.ajouter)
                {
                    tournee.DelLivraison(UndoStruct.carte, UndoStruct.livraison, UndoStruct.index);
                    // add RedoPushUndoAdd
                }
                else if (UndoStruct.action==ActionType.modifier)
                {
                    //TODO
                    // add RedoPushUndoModify
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
                    tournee.DelLivraison(RedoStruct.carte, RedoStruct.livraison, RedoStruct.index);
                    // add RedoPushUndoDelete
                }
                else if (RedoStruct.action == ActionType.ajouter)
                {
                    tournee.AddLivraison(RedoStruct.carte, RedoStruct.livraison, RedoStruct.index);
                    // add RedoPushUndoAdd
                }
                else if (RedoStruct.action == ActionType.modifier)
                {
                    //TODO
                    // add RedoPushUndoModify
                }

            }

        }
    }
}
