import { createAction } from 'redux-act'



export const loadManage = createAction('loadManage')
export const receiveUserList = createAction('receiveUserList')

export const loadSystemStatus = createAction('loadSystemStatus')
export const receiveSystemStatusData = createAction('receiveSystemStatusData')

export const clearDateAction = createAction('clearDateAction')
// export const pulBasicDataAction = createAction('pulBasicDataAction')
export const receiveConnectionAction = createAction('receiveConnectionAction')
export const ariseSystemEventAction = createAction('ariseSystemEventAction')