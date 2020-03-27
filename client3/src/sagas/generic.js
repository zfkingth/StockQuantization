import { select, put, call, take, spawn, delay } from 'redux-saga/effects'
import { eventChannel } from 'redux-saga';

import { removeStateReceivedFrom } from '../actions/cache';
import * as signalR from '@aspnet/signalr'
import * as stockActions from '../actions/stock'
import * as manageActions from '../actions/manage';

const enters = {
  // yourStories: function* (state) {
  //   yield put(selectTab(state.yourStories.tab))
  // },
  // story: function* (state) {
  //   const storyId = state.navigation.storyId
  //   const story = yield callWith401Handle(get, STORY_DETAIL(storyId))
  //   yield put(receiveStory(story))
  // },
  // stories: function* (state) {
  //   const { stories } = yield callWith401Handle(get, STORIES)
  //   yield put(receiveStories(stories))
  // },
  stock: function* (state) {

  }

}


const NotificationEnum=
{
   TaskStart:0,
   TaskProgress:1,
   TaskFail:2,
   TaskSuccess:3,
}

export function* enterPage() {
  const state = yield select()
  const pageName = state.navigation.page
  const entersFunc = enters[pageName]
  if (entersFunc) yield entersFunc(state)
}

function* listenNotifications() {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl(process.env.REACT_APP_SingalR_URL, { accessTokenFactory: () => localStorage.token })
    .build()

  let attempt = 0
  let connected = false
  while (attempt < 5 && !connected) {
    attempt++
    connected = true
    try {
      yield call(() => connection.start())
      console.info('SignalR: successfully connected')
    } catch (err) {
      console.info(`SignalR: attempt ${attempt}: failed to connect`)
      yield delay(1000)
      connected = false
    }
  }

  if (connected) {
    const getEventChannel = connection => eventChannel(emit => {
      const handler = data => { emit(data) };
      connection.on('notification', handler);
      return () => { connection.off() };
    });

    yield put(manageActions.receiveConnectionAction(connection));
    const channel = yield call(getEventChannel, connection);
    while (true) {
      const { notificationType, payload } = yield take(channel);
      // if (['LIKE', 'UNLIKE'].includes(notificationType)) {
      //   const message = `${payload.username} ${notificationType.toLowerCase()}d "${payload.storyTitle}"`
      //   yield put(toggleSnackbar(message))
      // } else if (notificationType === 'SHARE') {
      //   const message = `${payload.username} invited you to edit his story: "${payload.storyTitle}"`
      //   yield put(toggleSnackbar(message))
      // } else if (notificationType === 'STORY_EDIT') {
      //   const { navigation, editor } = yield select()
      //   if (navigation.page === 'editor' && editor.storyId === payload.id) {
      //     yield put(updateStory(payload))
      //   }
      // }
      console.log('receive notification:' + notificationType);
      console.log('receive payload:' + JSON.stringify(payload));
      if (notificationType ===  NotificationEnum.TaskStart ) {

        yield put(stockActions.TaskStart(payload));
      }

      else if (notificationType === NotificationEnum.TaskSuccess) {


        yield put(stockActions.TaskSuccess(payload));
      }

      else if (notificationType ===  NotificationEnum.TaskProgress ) {


        yield put(stockActions.TaskProgress(payload));
      }
      else if (notificationType ===  NotificationEnum.TaskFail ) {

        yield put(stockActions.TaskFail(payload));
      }

    }
  }
}

export function* startApp() {
  window.history.pushState({}, '', '')

  yield spawn(listenNotifications)

  // function* ticking() {
  //   yield put(tickAction())
  //   yield delay(TICK)
  //   yield* ticking()
  // }
  // yield* ticking()
}

const exits = {
  // editor: function* () {
  //   yield put(clearEditor())
  // },
  // yourStories: function* () {
  //   yield put(clearYourStories())
  // },
  story: function* () {
    yield put(removeStateReceivedFrom('story'))
  }
}

export function* exitPage({ payload }) {
  const state = yield select()

  const exitsFunc = exits[payload]
  if (exitsFunc) yield exitsFunc(state)
}

export function* tick() {
  // const { navigation: { page } } = yield select()
  // if (page === 'editor') {
  //   const { editor: { lastSave, lastEdit, saving } } = yield select()
  //   if (!saving && lastEdit && lastEdit > lastSave && Date.now() - lastSave > SAVE_PERIOD) {
  //     yield put(save())
  //   }
  // }
}
