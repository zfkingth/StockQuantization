import { getFormSubmitErrors } from 'redux-form'

import * as signalR from '@microsoft/signalr'

export function transFormValuestoPostValues(formValues, defaultValues, stockList, baseDate) {
  let ret = {};
  ret.baseDate = baseDate;
  for (let key in defaultValues) {
    ret[key] = formValues[key];
  }
  ret.StockIdList = stockList.map(function (item, index, array) {
    return item.stockId;
  });

  return ret;
}


export function transFormValuestoPostValuesWithDateList(formValues, defaultValues, stockList, baseDate) {
  let ret = {};
  ret.baseDate = baseDate;
  for (let key in defaultValues) {
      ret[key] = formValues[key];
  }
  ret.StockIdList = stockList.map(function (item, index, array) {
      return item.stockId;
  });
  ret.dateList = stockList.map(function (item, index, array) {
      return item.date;
  });

  return ret;
}

export function transSubmissonToString(state, formName) {
  const obj = getFormSubmitErrors(formName)(state);
  const entries = Object.entries(obj);
  const ret = entries.reduce(function (prev, cur) {
    return prev + cur[1];
  }, '');

  return ret;
}


export const submitHandler = (onSubmit, enabledSubmit) => e => {
  e.preventDefault()
  if (enabledSubmit) onSubmit()
}

export const submitAsyncValidation = (
  handleSubmit,
  enabledSubmit,
  onSubmit
) => e => {
  e.preventDefault()
  enabledSubmit &&
    handleSubmit(
      values =>
        new Promise(
          (resolve, reject) =>
            onSubmit({ values, resolve, reject })
        )
    )(e)
}

export const isValid = (state, formName) => isFormValid(state.form[formName])

export const isFormValid = form => form && !form.syncErrors && !form.pristine

export const isConnected = (state) => state.manage.connection && state.manage.connection.state === signalR.HubConnectionState.Connected;
