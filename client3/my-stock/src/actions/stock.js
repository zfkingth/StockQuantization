import { createAction } from 'redux-act'

export const receiveStockSearchResult = createAction('receiveStockSearchResult')


export const changeBaseDate = createAction('changeBaseDate')

export const searchUpwardGap = createAction('searchUpwardGap')
export const searchColseBreak = createAction('searchColseBreak')
export const searchUpMA = createAction('searchUpMA')
export const searchApproach = createAction('searchApproach')
export const searchCirculatedMarket = createAction('searchCirculatedMarket')
export const searchTurnOverRate = createAction('searchTurnOverRate')
export const searchExceptZhangFu = createAction('searchExceptZhangFu')
export const searchNRiseOpen = createAction('searchNRiseOpen')
export const searchVolumeDecrease = createAction('searchVolumeDecrease')
export const searchVolumeBreak = createAction('searchVolumeBreak')






export const stockRequestStart = createAction('stockRequestStart')
export const stockRequestEnd = createAction('stockRequestEnd')
export const TaskStart = createAction('TaskStart')
export const TaskProgress = createAction('TaskProgress')
export const TaskFail = createAction('TaskFail')
export const TaskSuccess = createAction('TaskSuccess')