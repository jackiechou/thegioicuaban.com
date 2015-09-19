using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace CommonLibrary.UI.WebControls.Common
{
    public class NodeImage : IStateManager
    {
        private bool _marked = false;
        private StateBag _state;

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [jbrinkman]     5/6/2004        Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public NodeImage()
        {
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///
        /// </summary>
        /// <param name="NewImageUrl"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [jbrinkman]     5/6/2004        Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public NodeImage(string NewImageUrl)
        {
            if (ImageUrl == null)
            {
                throw new ArgumentNullException();
            }
            ((IStateManager)this).TrackViewState();
            ImageUrl = NewImageUrl;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [jbrinkman]     5/6/2004        Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public bool IsTrackingViewState
        {
            get { return _marked; }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [jbrinkman]     5/6/2004        Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void TrackViewState()
        {
            _marked = true;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [jbrinkman]     5/6/2004        Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public object SaveViewState()
        {
            // save _state state
            object _stateState = null;
            if ((_state != null))
            {
                _stateState = ((IStateManager)_state).SaveViewState();
            }
            return _stateState;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///
        /// </summary>
        /// <param name="state"></param>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [jbrinkman]     5/6/2004        Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void LoadViewState(object state)
        {
            if ((state != null))
            {
                ((IStateManager)ViewState).LoadViewState(state);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [jbrinkman]     5/6/2004        Created
        ///     [cnurse]    11/3/2004   Fixed to work under Option Strict On
        /// </history>
        /// -----------------------------------------------------------------------------
        public string ImageUrl
        {
            get
            {
                string _imageUrl = string.Empty;
                if ((ViewState["ImageUrl"] == null) == false)
                {
                    _imageUrl = (string)ViewState["ImageUrl"];
                }
                return _imageUrl;
            }
            set { ViewState["ImageUrl"] = value; }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [jbrinkman]     5/6/2004        Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected StateBag ViewState
        {
            get
            {
                if (_state == null)
                {
                    _state = new StateBag(true);
                    if (IsTrackingViewState)
                    {
                        ((IStateManager)_state).TrackViewState();
                    }
                }
                return _state;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [jbrinkman]     5/6/2004        Created
        /// </history>
        /// -----------------------------------------------------------------------------
        internal void SetDirty()
        {
            if ((_state != null))
            {
                foreach (string key in _state.Keys)
                {
                    _state.SetItemDirty(key, true);
                }
            }
        }
    }

}
