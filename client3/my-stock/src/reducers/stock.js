import { createReducer } from 'redux-act'

import * as stockActions from '../actions/stock'
import '../utils/api';

const getDefaultState = () => ({

  stockList: [],
  isIdle: true,
  completed: 0,
  taskInfo: '',
  baseDate: getNextMonthDateText(),

})

const getNextMonthDateText = () => {
  const temp = new Date();
  temp.setMonth(temp.getMonth() + 1);



  return temp.Format("yyyy-MM-dd");
}

export default _ =>
  createReducer(
    {
      [stockActions.changeBaseDate]: (state, baseDate) => ({
        ...state,
        //使用字面量
        baseDate,
      }),
      [stockActions.stockRequestStart]: (state, p) => ({
        ...state,
        isIdle: false,
      }),
      [stockActions.stockRequestEnd]: (state, p) => ({
        ...state,
        isIdle: true,
      }),
      [stockActions.TaskStart]: (state, payload) => ({
        ...state,
        taskInfo: '任务开始。',
        completed: 0,
      }),

      [stockActions.TaskFail]: (state, payload) => ({
        ...state,
        taskInfo: '任务失败：' + payload.message,
        stockList: payload.result,
      }),
      [stockActions.TaskSuccess]: (state, payload) => ({
        ...state,
        taskInfo: '任务成功,找到' + payload.result.length + '股票',
        stockList: payload.result,
        completed: 100,
      }),
      [stockActions.TaskProgress]: (state, payload) => ({
        ...state,
        taskInfo: '任务进行中......',
        completed: payload.progress,
      }),
    },
    getDefaultState()
  )
