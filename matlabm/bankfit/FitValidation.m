function [allTQ,allSQ,allRatio,allIdx,allFit] = FitValidation(x,vx,wind,waveIdx)
% 库光谱拟合法交互验证
% 输入：x，库光谱，pr，库光谱性质，wind，判断拟合是否充分进行移动相关系数和识别参数计算的窗口宽度，%vx，验证集光谱，vy，验证集性质
%wave1,判断拟合是否充分进行移动相关系数和识别参数计算的光谱区间初始位置，wave2，判断拟合是否充分进行移动相关系数和识别参数计算的光谱区间终了位置，
% 输出：result，性质显示，minSQ，最小移动相关系数,TQ，识别参数，
% result：（300+1）*性质：第1行为待识别样品的性质，第2行至非零数字行，为识别出来的库样品的性质，其他依次类推
% result2：每一列从第一行到非零数字行，为参与拟合的库样品的比例
% minSQ，1*校正集样品数目
% TQ，1*校正集样品数目
	xcount = size(x,2);
    [m,n] = size(vx);
	allTQ = zeros(n,1);
	allSQ = zeros(n,1);
	allFit = zeros(m,n);
    allRatio = zeros(xcount,n);
    allIdx = zeros(xcount,n);
    maxlen = 0;
	for ii=1:n
		y = vx(:,ii);
		[TQ, SQ, ratio, idx, fit ] = FitPredictor(x,y,wind,waveIdx,1);
		allTQ(ii) = TQ;
		allSQ(ii) = SQ; 
		allRatio(1:length(ratio),ii) = ratio;
		allIdx(1:length(ratio),ii) = idx;
		allFit(:,ii) = fit;
        if maxlen<length(ratio)
            maxlen = length(ratio);    
        end
	end
    if maxlen>0
        allRatio = allRatio(1:maxlen,:);
        allIdx = allIdx(1:maxlen,:);
    end
end