import React from 'react';
import FormControlLabel from '@material-ui/core/FormControlLabel';
import Switch from '@material-ui/core/Switch';

import PropTypes from 'prop-types';
import { withStyles } from '@material-ui/core/styles';
import TextField from '@material-ui/core/TextField';
import Button from '@material-ui/core/Button';
import { connectTo } from '../../utils/generic';
import { searchApproach as searchAction, changeBaseDate } from '../../actions/stock';
import { submitAsyncValidation, transFormValuestoPostValues, isConnected } from '../../utils/forms'
import _ from 'lodash'
import { FormLabel } from '@material-ui/core';

const styles = theme => ({
    container: {
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'flex-start',
        flexWrap: 'wrap',


    },
    textField: {
        marginLeft: theme.spacing.unit,
        marginRight: theme.spacing.unit,
        width: 220,
    },
    switch: {

        marginLeft: theme.spacing.unit,
    },

    formTextInput: {
        fontSize: 18
    },
    formTextLabel: {
        fontSize: 18
    },

    dense: {
        marginTop: 19,
    },
    menu: {
        width: 220,
    },
    button: {
        margin: theme.spacing.unit,
    },
});

const defaultValues = {
    xiangchaLv: 2,
    huiTiaoFuDuLow: 9.5,
    huiTiaoFuDuHigh: 25.0,
    nearDaysNum: 1,
    circleDaysNum: 60,
    searchFromAllStocks: false,
};

class TextFields extends React.PureComponent {

    state = {
        ...defaultValues,
        formHasError: false,
        formErrorMessage: '',
    };

    handleChange = name => event => {
        this.setState({ [name]: parseFloat(event.target.value) });
    };
    switchChange = name => event => {
        this.setState({ [name]: event.target.checked });
    };

    handleSubmit = fun => event => {
        const postModel = transFormValuestoPostValues(this.state, defaultValues, this.props.stockList, this.props.baseDate);
        const promise = fun(postModel);
        promise.then(data => {
            //请求成功
            this.setState({
                formHasError: false,
                formErrorMessage: '',
            });
        },
            //请求失败 reject的回调
            message => {

                this.setState({
                    formHasError: true,
                    formErrorMessage: _.toString(message),
                });

            }

        )
    };

    render() {
        const { classes } = this.props;

        return (
            <form className={classes.container} noValidate autoComplete="off"
                onSubmit={
                    submitAsyncValidation(
                        this.handleSubmit,
                        this.props.enabledSubmit,
                        this.props.searchAction,
                    )}
            >
                <label className={classes.textField} >股价接近平台高点，但尚未突破。</label>
                <TextField required
                    id='t1'
                    label="以__个交易日为周期"
                    value={this.state.circleDaysNum}
                    onChange={this.handleChange('circleDaysNum')}
                    type="number"
                    className={classes.textField}
                    InputProps={{
                        classes: {
                            input: classes.formTextInput
                        }
                    }}
                    InputLabelProps={{
                        classes: {
                            root: classes.formTextLabel
                        }
                    }}
                    variant="outlined"
                    margin="normal"
                />

                <TextField required
                    id='t2'
                    label="在最近__天内"
                    value={this.state.nearDaysNum}
                    onChange={this.handleChange('nearDaysNum')}
                    type="number"
                    className={classes.textField}
                    InputProps={{
                        classes: {
                            input: classes.formTextInput
                        }
                    }}
                    InputLabelProps={{
                        classes: {
                            root: classes.formTextLabel
                        }
                    }}
                    variant="outlined"
                    margin="normal"
                />

                <TextField required
                    id='t3'
                    label="股价与平台前期高点相差在__%内以内"
                    value={this.state.xiangchaLv}
                    onChange={this.handleChange('xiangchaLv')}
                    type="number"
                    className={classes.textField}
                    InputProps={{
                        classes: {
                            input: classes.formTextInput
                        }
                    }}
                    InputLabelProps={{
                        classes: {
                            root: classes.formTextLabel
                        }
                    }}
                    variant="outlined"
                    margin="normal"
                />


                <TextField required
                    id='t4'
                    label="回调幅度不低于__%"
                    value={this.state.huiTiaoFuDuLow}
                    onChange={this.handleChange('huiTiaoFuDuLow')}
                    type="number"
                    className={classes.textField}
                    InputProps={{
                        classes: {
                            input: classes.formTextInput
                        }
                    }}
                    InputLabelProps={{
                        classes: {
                            root: classes.formTextLabel
                        }
                    }}
                    margin="normal"
                    variant="outlined"
                />

                <TextField required
                    id='t5'
                    label="回调幅度不高于__%"
                    value={this.state.huiTiaoFuDuHigh}
                    onChange={this.handleChange('huiTiaoFuDuHigh')}
                    type="number"
                    className={classes.textField}
                    InputProps={{
                        classes: {
                            input: classes.formTextInput
                        }
                    }}
                    InputLabelProps={{
                        classes: {
                            root: classes.formTextLabel
                        }
                    }}
                    margin="normal"
                    variant="outlined"
                />
                <TextField required
                    id='baseDate'
                    label="搜索基准日期之前的数据"
                    value={this.props.baseDate}
                    onChange={(e) => this.props.changeBaseDate(e.target.value)}
                    type="date"
                    className={classes.textField}
                    InputProps={{
                        classes: {
                            input: classes.formTextInput
                        }
                    }}
                    InputLabelProps={{
                        classes: {
                            root: classes.formTextLabel
                        }
                    }}
                    margin="normal"
                    variant="outlined"
                />

                <FormControlLabel
                    control={
                        <Switch
                            className={classes.switch}
                            id='searchFromAllStocks'
                            checked={this.state.searchFromAllStocks}
                            onChange={this.switchChange('searchFromAllStocks')}

                        />
                    }
                    label="从所有股票列表中筛选"
                />


                <Button type="submit" disabled={!this.props.enabledSubmit} variant="contained" color="primary" className={classes.button}>
                    搜索股票
                 </Button>
                <FormLabel disabled={!this.state.formHasError}
                    className={classes.menu}
                    error={this.state.formHasError}>
                    {this.state.formErrorMessage}
                </FormLabel>


            </form>
        );
    }
}

TextFields.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default connectTo(
    state => ({
        enabledSubmit: state.stock.isIdle && isConnected(state),
        stockList: state.stock.stockList,
        baseDate: state.stock.baseDate,
    }),
    { searchAction, changeBaseDate },
    withStyles(styles)(TextFields)
);




