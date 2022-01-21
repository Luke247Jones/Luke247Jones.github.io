import React, { Component } from 'react';
import { TouchableOpacity, Text, TextInput, StyleSheet, View } from 'react-native';
import { Container, Header, Content, Form, Item, Input, Label, DatePicker, Picker, Icon } from 'native-base';

export default class Success extends Component {
  render() {
    return (
      <View style= {styles.container}>
        <Text style= {styles.text}> Success </Text>
      </View>
    )
  }
}

const styles = StyleSheet.create({
    container: {
      flex: 1,
      justifyContent: 'center',
      alignItems: 'center',
      alignContent: 'center',
      backgroundColor: "#eeeeee",
    },
    text: {
      color: 'green',
      fontSize: 75,
      fontWeight: 'bold'
    }
});
