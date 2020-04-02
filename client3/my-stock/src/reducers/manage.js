import { createReducer } from 'redux-act'

import * as actions from '../actions/manage'

const getDefaultState = () => ({
    userList:[],
    systemStatus:undefined,
    connection:undefined,
})

export default _ =>
  createReducer(
    {
      [actions.receiveUserList]: (state, payload) => ({
        ...state,
        userList: payload,
      }),
      [actions.receiveSystemStatusData]: (state, payload) => ({
        ...state,
        systemStatus: payload,
      }),
      [actions.receiveConnectionAction]: (state, payload) => ({
        ...state,
        connection: payload,
      }),
       
    },
    getDefaultState()
  )
