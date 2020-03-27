
import React from "react";


import { connectTo } from '../../utils/generic';






class tempcontrol extends React.PureComponent {
    render() {
        return (
            <h1>系统当前正在执行的任务个数为： {this.props.taskNum}</h1>
        )
    }
}


export default connectTo(
    state => ({
        taskNum: state.manage.systemStatus.num,
    }),
    {},
    tempcontrol
);
