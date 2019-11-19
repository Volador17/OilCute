function [TQ, SQ, ratio, idx, fit ] = FitPredictor(x,y,wind,waveIdx,topk)
% 库光谱拟合法预测未知样品
% 输入：x，库光谱，pr，库光谱性质，wind，判断拟合是否充分进行移动相关系数和识别参数计算的窗口宽度，%vx，未知样品光谱
%wave1,判断拟合是否充分进行移动相关系数和识别参数计算的光谱区间初始位置，wave2，判断拟合是否充分进行移动相关系数和识别参数计算的光谱区间终了位置，
% 输出：result，性质显示，minSQ，最小移动相关系数,TQ，识别参数，
% result：（300+1）*性质：第1行为待识别样品的性质，第2行至非零数字行，为识别出来的库样品的性质，其他依次类推
% result2：每一列从第一行到非零数字行，为参与拟合的库样品的比例
% minSQ，1*校正集样品数目
% TQ，1*校正集样品数目

	[ratio,idx] = bankfit(x,y);
	nx = x(:,idx);
	fit= nx * ratio;
	[TQ,SQ] = mwcorrVector(fit(waveIdx,:),y(waveIdx,:),wind,topk);
    
    SQ = min(SQ);
end