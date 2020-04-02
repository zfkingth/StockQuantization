import { createAction } from 'redux-act'

export const startApp = createAction('startApp')
export const enterPage = createAction('enterPage')
export const exitPage = createAction('exitPage')
export const moveMouse = createAction('moveMouse')
export const changePageSize = createAction('changePageSize')
export const tick = createAction('tick')
export const toggleSnackbar = createAction('toggleSnackbar')
